using Nest;
using Z.Commons;
using Z.SearchService.Domain;

namespace Z.SearchService.Infrastructure
{
    public class SearchRepository : ISearchRepository
    {
        private readonly IElasticClient elasticClient;

        public SearchRepository(IElasticClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }

        /// <summary>
        /// 根据集Id删除数据
        /// </summary>
        /// <param name="episodeId"></param>
        /// <returns></returns>
        public Task DeleteAsync(Guid episodeId)
        {
            elasticClient.DeleteByQuery<Episode>(q => q
                .Index("episodes")
                .Query(rq => rq
                    .Term(f => f.Id, "elasticsearch.pm")));
            //因为有可能文档不存在，所以不检查结果
            //如果Episode被删除，则把对应的数据也从Elastic Search中删除
            return elasticClient.DeleteAsync(new DeleteRequest("episodes", episodeId));
        }

        /// <summary>
        /// 根据关键词、页引索、页数搜索集
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="pageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public async Task<SearchEpisodesResponse> SearchEpisodes(string keyWord, int pageIndex, int PageSize)
        {
            int from = PageSize * (pageIndex - 1);
            string kw = keyWord;
            Func<QueryContainerDescriptor<Episode>, QueryContainer> query = (q) =>
                q.Match(mq => mq.Field(f => f.CnName).Query(kw))
                || q.Match(mq => mq.Field(f => f.EngName).Query(kw))
                || q.Match(mq => mq.Field(f => f.PlainSubtitle).Query(kw));
            Func<HighlightDescriptor<Episode>, IHighlight> highlightSelector = h => h
                .Fields(fs => fs.Field(f => f.PlainSubtitle));
            var result = await this.elasticClient.SearchAsync<Episode>(s => s.Index("episodes")
                .From(from)
                .Size(PageSize)
                .Query(query)
                .Highlight(highlightSelector));

            if (!result.IsValid)
            {
                throw result.OriginalException;
            }
            List<Episode> episodes = new List<Episode>();
            foreach (var hit in result.Hits)
            {
                string highlightedSubtitle;
                //如果没有预览内容，则显示前50个字
                if (hit.Highlight.ContainsKey("plainSubtitle"))
                {
                    highlightedSubtitle = string.Join("\r\n", hit.Highlight["plainSubtitle"]);
                }
                else
                {
                    highlightedSubtitle = hit.Source.PlainSubtitle.Cut(50);
                }
                var episode = hit.Source with { PlainSubtitle = highlightedSubtitle };
                episodes.Add(episode);
            }
            return new SearchEpisodesResponse(episodes, result.Total);
        }

        /// <summary>
        /// 将集插入，进行更新
        /// </summary>
        /// <param name="episode"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task UpsertAsync(Episode episode)
        {
            var response = await elasticClient.IndexAsync(episode, idx => idx.Index("episodes").Id(episode.Id));//Upsert:Update or Insert
            if (!response.IsValid)
            {
                throw new ApplicationException(response.DebugInformation);
            }
        }
    }
}

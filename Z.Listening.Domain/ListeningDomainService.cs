using Z.Commons;
using Z.DomainCommons.Models;
using Z.Listening.Domain.Entities;

namespace Z.Listening.Domain
{
    public class ListeningDomainService
    {
        private readonly IListeningRepository repository;

        public ListeningDomainService(IListeningRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// 增加专辑(返回一个专辑实体，不没有进行数据插入操作)
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Album> AddAlbumAsync(Guid categoryId, MultilingualString name)
        {
            int maxSeq = await repository.GetMaxSeqOfAlbumsAsync(categoryId);
            var id = Guid.NewGuid();
            return Album.Create(id, maxSeq + 1, name, categoryId);
        }

        /// <summary>
        /// 排序专辑
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="sortedAlbumIds"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task SortAlbumsAsync(Guid categoryId, Guid[] sortedAlbumIds)
        {
            var albums = await repository.GetAlbumsByCategoryIdAsync(categoryId);
            var idsInDB = albums.Select(a => a.Id);
            if (!idsInDB.SequenceIgnoredEqual(sortedAlbumIds))
            {
                throw new Exception($"提交的待排序Id中必须是categoryId={categoryId}分类下所有的Id");
            }

            int seqNum = 1;
            //一个in语句一次性取出来更快，不过在非性能关键节点，业务语言比性能更重要
            foreach (Guid albumId in sortedAlbumIds)
            {
                var album = await repository.GetAlbumByIdAsync(albumId);
                if (album == null)
                {
                    throw new Exception($"albumId={albumId}不存在");
                }
                album.ChangeSequenceNumber(seqNum);//顺序改序号
                seqNum++;
            }
        }

        /// <summary>
        /// 增加类别(返回一个类别实体，不没有进行数据插入操作)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="coverUrl"></param>
        /// <returns></returns>
        public async Task<Category> AddCategoryAsync(MultilingualString name, Uri coverUrl)
        {
            int maxSeq = await repository.GetMaxSeqOfCategoriesAsync();
            var id = Guid.NewGuid();
            return Category.Create(id, maxSeq + 1, name, coverUrl);
        }

        /// <summary>
        /// 排序类别
        /// </summary>
        /// <param name="sortedCategoryIds"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task SortCategoriesAsync(Guid[] sortedCategoryIds)
        {
            var categories = await repository.GetCategoriesAsync();
            var idsInDB = categories.Select(a => a.Id);
            if (!idsInDB.SequenceIgnoredEqual(sortedCategoryIds))
            {
                throw new Exception("提交的待排序Id中必须是所有的分类Id");
            }
            int seqNum = 1;
            //一个in语句一次性取出来更快，不过在非性能关键节点，业务语言比性能更重要
            foreach (Guid catId in sortedCategoryIds)
            {
                var cat = await repository.GetCategoryByIdAsync(catId);
                if (cat == null)
                {
                    throw new Exception($"categoryId={catId}不存在");
                }
                cat.ChangeSequenceNumber(seqNum);//顺序改序号
                seqNum++;
            }
        }

        /// <summary>
        /// 增加集(返回一个类别实体，不没有进行数据插入操作)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="albumId"></param>
        /// <param name="audioUrl"></param>
        /// <param name="durationInSecond"></param>
        /// <param name="subtitleType"></param>
        /// <param name="subtitle"></param>
        /// <returns></returns>
        public async Task<Episode> AddEpisodeAsync(MultilingualString name,
            Guid albumId, Uri audioUrl, double durationInSecond,
            string subtitleType, string subtitle)
        {
            int maxSeq = await repository.GetMaxSeqOfEpisodesAsync(albumId);
            var id = Guid.NewGuid();
            /*
            Episode episode = Episode.Create(id, maxSeq + 1, name, albumId,
                audioUrl,durationInSecond, subtitleType, subtitle);*/
            var builder = new Episode.Builder();
            builder.Id(id)
                .SequenceNumber(maxSeq + 1)
                .Name(name)
                .AlbumId(albumId)
                .AudioUrl(audioUrl)
                .DurationInSecond(durationInSecond)
                .SubtitleType(subtitleType)
                .Subtitle(subtitle);
            return builder.Build();
        }

        /// <summary>
        /// 排序集
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="sortedEpisodeIds"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task SortEpisodesAsync(Guid albumId, Guid[] sortedEpisodeIds)
        {
            var episodes = await repository.GetEpisodesByAlbumIdAsync(albumId);
            var idsInDB = episodes.Select(a => a.Id);
            if (!sortedEpisodeIds.SequenceIgnoredEqual(idsInDB))
            {
                throw new Exception($"提交的待排序Id中必须是albumId={albumId}专辑下所有的Id");
            }

            int seqNum = 1;
            foreach (Guid episodeId in sortedEpisodeIds)
            {
                var episode = await repository.GetEpisodeByIdAsync(episodeId);
                if (episode == null)
                {
                    throw new Exception($"episodeId={episodeId}不存在");
                }
                episode.ChangeSequenceNumber(seqNum);//顺序改序号
                seqNum++;
            }
        }
    }
}

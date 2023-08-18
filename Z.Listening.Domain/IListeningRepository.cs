using Z.Listening.Domain.Entities;

namespace Z.Listening.Domain
{
    public interface IListeningRepository
    {
        /// <summary>
        /// 根据id获取类别
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public Task<Category?> GetCategoryByIdAsync(Guid categoryId);

        /// <summary>
        /// 获取类别
        /// </summary>
        /// <returns></returns>
        public Task<Category[]> GetCategoriesAsync();

        /// <summary>
        /// 获取类别最大序号
        /// </summary>
        /// <returns></returns>
        public Task<int> GetMaxSeqOfCategoriesAsync();

        /// <summary>
        /// 根据id获取专辑
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns></returns>
        public Task<Album?> GetAlbumByIdAsync(Guid albumId);

        /// <summary>
        /// 获取专辑最大序号
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public Task<int> GetMaxSeqOfAlbumsAsync(Guid categoryId);

        /// <summary>
        /// 根据类别id获取专辑
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public Task<Album[]> GetAlbumsByCategoryIdAsync(Guid categoryId);

        /// <summary>
        /// 根据id获取集
        /// </summary>
        /// <param name="episodeId"></param>
        /// <returns></returns>
        public Task<Episode?> GetEpisodeByIdAsync(Guid episodeId);

        /// <summary>
        /// 获取集最大序号
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns></returns>
        public Task<int> GetMaxSeqOfEpisodesAsync(Guid albumId);

        /// <summary>
        /// 根据专辑获取集
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns></returns>
        public Task<Episode[]> GetEpisodesByAlbumIdAsync(Guid albumId);
    }
}

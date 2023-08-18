using Z.Commons.Validators;
using Z.Listening.Main.WebAPI.Controllers.Albums.ViewModels;

namespace Z.Listening.Main.WebAPI.Controllers.Albums
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly IListeningRepository repository;
        private readonly IMemoryCacheHelper cacheHelper;

        public AlbumController(IListeningRepository repository, IMemoryCacheHelper cacheHelper)
        {
            this.repository = repository;
            this.cacheHelper = cacheHelper;
        }

        /// <summary>
        /// 根据id查找专辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<AlbumVM>> FindById([RequiredGuid] Guid id)
        {
            var album = await cacheHelper.GetOrCreateAsync($"AlbumController.FindById.{id}", async (e) => AlbumVM.Create(await repository.GetAlbumByIdAsync(id)));
            if (album == null)
            {
                return NotFound();
            }
            return album;
        }

        /// <summary>
        /// 根据类别id进行查找
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{categoryId}")]
        public async Task<ActionResult<AlbumVM[]>> FindByCategoryId([RequiredGuid] Guid categoryId)
        {
            //写到单独的local函数的好处是避免回调中代码太复杂
            Task<Album[]> FindDataAsync()
            {
                return repository.GetAlbumsByCategoryIdAsync(categoryId);
            }

            var task = cacheHelper.GetOrCreateAsync($"AlbumController.FindByCategoryId.{categoryId}", async (e) => AlbumVM.Create(await FindDataAsync()));
            return await task;
        }
    }
}

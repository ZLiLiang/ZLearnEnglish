using Microsoft.AspNetCore.Authorization;
using Z.Commons.Validators;
using Z.Listening.Admin.WebAPI.Albums.Request;
using Z.Listening.Infrastructure;

namespace Z.Listening.Admin.WebAPI.Albums
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [UnitOfWork(typeof(ListeningDbContext))]
    public class AlbumController : ControllerBase
    {
        private readonly ListeningDbContext dbContext;
        private IListeningRepository repository;
        private readonly ListeningDomainService domainService;

        public AlbumController(ListeningDbContext dbContext, IListeningRepository repository, ListeningDomainService domainService)
        {
            this.dbContext = dbContext;
            this.repository = repository;
            this.domainService = domainService;
        }

        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Album?>> FindById([RequiredGuid] Guid id)
        {
            var album = await repository.GetAlbumByIdAsync(id);
            return album;
        }

        /// <summary>
        /// 根据类别id查询
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{categoryId}")]
        public Task<Album[]> FindByCategoryId([RequiredGuid] Guid categoryId)
        {
            return repository.GetAlbumsByCategoryIdAsync(categoryId);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Guid>> Add(AlbumAddRequest request)
        {
            Album album = await domainService.AddAlbumAsync(request.CategoryId, request.Name);
            dbContext.Add(album);
            return album.Id;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Update([RequiredGuid] Guid id, AlbumUpdateRequest request)
        {
            var album = await repository.GetAlbumByIdAsync(id);
            if (album == null)
            {
                return NotFound("id没找到");
            }
            album.ChangeName(request.Name);
            return Ok();
        }

        /// <summary>
        /// 根据id删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteById([RequiredGuid] Guid id)
        {
            var album = await repository.GetAlbumByIdAsync(id);
            if (album == null)
            {
                //这样做仍然是幂等的，因为“调用N次，确保服务器处于与第一次调用相同的状态。”与响应无关
                return NotFound($"没有Id={id}的Album");
            }
            album.SoftDelete();//软删除
            return Ok();
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Hide([RequiredGuid] Guid id)
        {
            var album = await repository.GetAlbumByIdAsync(id);
            if (album == null)
            {
                return NotFound($"没有Id={id}的Album");
            }
            album.Hide();
            return Ok();
        }

        /// <summary>
        /// 展示
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Show([RequiredGuid] Guid id)
        {
            var album = await repository.GetAlbumByIdAsync(id);
            if (album == null)
            {
                return NotFound($"没有Id={id}的Album");
            }
            album.Show();
            return Ok();
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{categoryId}")]
        public async Task<ActionResult> Sort([RequiredGuid] Guid categoryId, AlbumsSortRequest req)
        {
            await domainService.SortAlbumsAsync(categoryId, req.SortedAlbumIds);
            return Ok();
        }
    }
}

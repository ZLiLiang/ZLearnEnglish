using Microsoft.AspNetCore.Authorization;
using Z.Commons.Validators;
using Z.Listening.Admin.WebAPI.Categories.Request;
using Z.Listening.Infrastructure;

namespace Z.Listening.Admin.WebAPI.Categories
{
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    [UnitOfWork(typeof(ListeningDbContext))]
    //供后台用的增删改查接口不用缓存
    public class CategoryController : ControllerBase
    {
        private IListeningRepository repository;
        private readonly ListeningDbContext dbContext;
        private readonly ListeningDomainService domainService;
        public CategoryController(ListeningDbContext dbContext, ListeningDomainService domainService, IListeningRepository repository)
        {
            this.dbContext = dbContext;
            this.domainService = domainService;
            this.repository = repository;
        }

        /// <summary>
        /// 查找所有
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<Category[]> FindAll()
        {
            return repository.GetCategoriesAsync();
        }

        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Category?>> FindById([RequiredGuid] Guid id)
        {
            //返回ValueTask的需要await的一下
            var cat = await repository.GetCategoryByIdAsync(id);
            if (cat == null)
            {
                return NotFound($"没有Id={id}的Category");
            }
            else
            {
                return cat;
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Guid>> Add(CategoryAddRequest req)
        {
            var category = await domainService.AddCategoryAsync(req.Name, req.CoverUrl);
            dbContext.Add(category);
            return category.Id;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Update([RequiredGuid] Guid id, CategoryUpdateRequest request)
        {
            var cat = await repository.GetCategoryByIdAsync(id);
            if (cat == null)
            {
                return NotFound("id不存在");
            }
            cat.ChangeName(request.Name);
            cat.ChangeCoverUrl(request.CoverUrl);
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
            var cat = await repository.GetCategoryByIdAsync(id);
            if (cat == null)
            {
                //这样做仍然是幂等的，因为“调用N次，确保服务器处于与第一次调用相同的状态。”与响应无关
                return NotFound($"没有Id={id}的Category");
            }
            cat.SoftDelete();//软删除
            return Ok();
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> Sort(CategoriesSortRequest req)
        {
            await domainService.SortCategoriesAsync(req.SortedCategoryIds);
            return Ok();
        }
    }
}

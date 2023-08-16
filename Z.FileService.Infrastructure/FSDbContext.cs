using Z.FileService.Domain.Entities;

namespace Z.FileService.Infrastructure
{
    public class FSDbContext : BaseDbContext
    {
        public DbSet<UploadedItem> UploadItems { get; private set; }

        public FSDbContext(DbContextOptions<FSDbContext> options, IMediator mediator)
            : base(options, mediator)
        {
        }

        /// <summary>
        /// 重写OnModelCreating，为modelBuilder添加当前实例所在的程序集
        /// </summary>
        /// <param name="modelBuilder">模型建造者</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}

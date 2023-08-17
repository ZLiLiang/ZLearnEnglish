using Microsoft.EntityFrameworkCore.Design;
using Z.CommonInitializer;
using Z.IdentityService.Infrastructure;

namespace Z.IdentityService.WebAPI
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdDbContext>
    {
        public IdDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = DbContextOptionsBuilderFactory.Create<IdDbContext>();
            return new IdDbContext(optionsBuilder.Options);
        }
    }
}

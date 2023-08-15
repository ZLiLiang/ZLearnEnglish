using Microsoft.EntityFrameworkCore;

namespace Z.CommonInitializer
{
    public class DbContextOptionsBuilderFactory
    {
        /// <summary>
        /// 返回包含slqserver连接的optionsBuilder
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static DbContextOptionsBuilder<TDbContext> Create<TDbContext>() where TDbContext : DbContext
        {
            var connStr = Environment.GetEnvironmentVariable("DefaultDB:ConnStr");
            var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
            //optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=YouzackVNextDB;User ID=sa;Password=dLLikhQWy5TBz1uM;");
            optionsBuilder.UseSqlServer(connStr);
            return optionsBuilder;
        }
    }
}

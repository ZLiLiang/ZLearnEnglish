using Microsoft.Extensions.Configuration;

namespace Z.AnyDBConfigProvider
{
    public class DBConfigurationSource : IConfigurationSource
    {
        private DBConfigOptions options;

        public DBConfigurationSource(DBConfigOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// 返回配置完毕的db配置提供者
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DBConfigurationProvider(options);
        }
    }
}

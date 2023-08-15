using Microsoft.Extensions.Configuration;
using System.Data;

namespace Z.AnyDBConfigProvider
{
    public static class DBConfigurationProviderExtensions
    {
        public static IConfigurationBuilder AddDbConfiguration(this IConfigurationBuilder builder, DBConfigOptions options)
        {
            return builder.Add(new DBConfigurationSource(options));
        }

        public static IConfigurationBuilder AddDbConfiguration(this IConfigurationBuilder builder, Func<IDbConnection> createDbConnection, string tableName = "T_Configs", bool reloadOnChange = false, TimeSpan? reloadInterval = null)
        {
            return AddDbConfiguration(builder, new DBConfigOptions
            {
                CreateDbConnection = createDbConnection,
                TableName = tableName,
                ReloadOnChange = reloadOnChange,
                ReloadInterval = reloadInterval
            });
        }
    }
}

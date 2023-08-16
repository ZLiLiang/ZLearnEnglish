using Microsoft.Extensions.DependencyInjection;
using Z.Commons;
using Z.FileService.Domain;
using Z.FileService.Infrastructure.Services;

namespace Z.FileService.Infrastructure
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IStorageClient, SMBStorageClient>();
            //services.AddScoped<IStorageClient, UpYunStorageClient>();
            services.AddScoped<IStorageClient, MockCloudStorageClient>();
            services.AddScoped<IFSRepository, FSRepository>();
            services.AddScoped<FSDomainService>();
            services.AddHttpClient();
        }
    }
}

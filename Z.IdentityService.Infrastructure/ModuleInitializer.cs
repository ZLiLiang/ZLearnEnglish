using Microsoft.Extensions.DependencyInjection;
using Z.Commons;
using Z.IdentityService.Domain;

namespace Z.IdentityService.Infrastructure
{
    class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<IdDomainService>();
            services.AddScoped<IIdRepository, IdRepository>();
        }
    }
}

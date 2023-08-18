using Microsoft.Extensions.DependencyInjection;
using Z.Commons;

namespace Z.Listening.Domain
{
    class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<ListeningDomainService>();
        }
    }
}

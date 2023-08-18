using Microsoft.Extensions.DependencyInjection;
using Z.Commons;
using Z.Listening.Domain;

namespace Z.Listening.Infrastructure
{
    class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<IListeningRepository, ListeningRepository>();
        }
    }
}

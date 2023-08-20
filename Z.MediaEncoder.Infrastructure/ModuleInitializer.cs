using Microsoft.Extensions.DependencyInjection;
using Z.Commons;
using Z.MediaEncoder.Domain;

namespace Z.MediaEncoder.Infrastructure
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<IMediaEncoderRepository, MediaEncoderRepository>();
            services.AddScoped<IMediaEncoder, ToM4AEncoder>();
        }
    }
}

using DocumentationCreator.Builders;
using DocumentationCreator.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentationCreator.Services
{
    public static class ServiceConfigurator
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<FileService>();
            services.AddSingleton<MarkdownBuilder>();
            services.AddSingleton<AiService>();
            services.AddSingleton<DocumentationService>();
            services.AddSingleton<ProjectOverviewBuilder>();
            services.AddSingleton<ArchitectureDiagramBuilder>();
            services.AddSingleton<IndexBuilder>();
            services.AddSingleton<ErDiagramBuilder>();
            services.AddSingleton<Run>();

            return services.BuildServiceProvider();
        }
    }
}

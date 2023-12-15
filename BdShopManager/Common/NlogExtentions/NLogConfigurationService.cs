using Common.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;

namespace Common.NlogExtentions
{
    public class NLogConfigurationService : IHostedService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ICurrentUserService _currentUserService;
        private readonly IExceptionRendererService _exceptionRendererService;

        public NLogConfigurationService(IHostingEnvironment hostingEnvironment, ICurrentUserService currentUserService, IExceptionRendererService exceptionRendererService)
        {
            _hostingEnvironment = hostingEnvironment;
            _currentUserService = currentUserService;
            _exceptionRendererService = exceptionRendererService;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {


            ConfigurationItemFactory.Default.CreateInstance = (Type type) =>
            {
                // Custom layout renderer.
                if (type == typeof(CustomExceptionLayoutRenderer))
                {
                    return new CustomExceptionLayoutRenderer(_hostingEnvironment, _currentUserService, _exceptionRendererService);
                }
                else
                    return Activator.CreateInstance(type); //default
            };

            LayoutRenderer.Register<CustomExceptionLayoutRenderer>("formatted-exception");
            LogManager.Configuration = LogManager.Configuration.Reload();
            LogManager.ReconfigExistingLoggers();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            LogManager.Flush();
            return Task.CompletedTask;
        }
    }
}

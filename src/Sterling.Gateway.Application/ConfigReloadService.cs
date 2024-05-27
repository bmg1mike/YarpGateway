using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sterling.Gateway.Application;

public class ConfigReloadService : BackgroundService
{
    private readonly CustomProxyConfigProvider _configProvider;
    private readonly ILogger<ConfigReloadService> _logger;

    public ConfigReloadService(CustomProxyConfigProvider configProvider, ILogger<ConfigReloadService> logger)
    {
        _configProvider = configProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            
            try
            {
                _configProvider.ReloadConfig();
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Adjust the interval as needed
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                
            }
        }
    }
}

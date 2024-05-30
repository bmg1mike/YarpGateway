using Yarp.ReverseProxy.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Sterling.Gateway.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Health;

namespace Sterling.Gateway.Application;

public class CustomProxyConfigProvider : IProxyConfigProvider
{
    private CustomProxyConfig _config;
    private CancellationChangeToken _changeToken;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CustomProxyConfigProvider> logger;

    public CustomProxyConfigProvider(IServiceScopeFactory serviceScopeFactory, ILogger<CustomProxyConfigProvider> logger)
    {
        // Load initial configuration
        this.logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _config = LoadConfig().Result;
        _changeToken = new CancellationChangeToken(new CancellationToken(false));

    }

    public IProxyConfig GetConfig() => _config;

    private async Task<CustomProxyConfig> LoadConfig()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            // var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var redis = scope.ServiceProvider.GetRequiredService<IRedisRepository>();

            var cacheKey = "YarpProxyConfig";
            var cachedConfig = await redis.Get(cacheKey);//_cache.GetStringAsync(cacheKey);
            //await redis.Delete(cacheKey);
            if (cachedConfig != null)
            {
                return JsonConvert.DeserializeObject<CustomProxyConfig>(cachedConfig)!;
            }

            var routesTask = Task.Run(async () =>
               {
                   using var routeScope = _serviceScopeFactory.CreateScope();
                   var context = routeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                   return await context.RouteConfigs.AsNoTracking().ToListAsync();
               });

            var clustersTask = Task.Run(async () =>
            {
                using var clusterScope = _serviceScopeFactory.CreateScope();
                var context = clusterScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                return await context.ClusterConfigs.AsNoTracking().ToListAsync();
            });

            await Task.WhenAll(routesTask, clustersTask);

            var routes = await routesTask;
            var clusters = await clustersTask;

            var routeConfigs = routes.Select(r => new RouteConfig
            {
                RouteId = r.RouteId,
                ClusterId = r.ClusterId,
                Match = new RouteMatch
                {
                    Path = r.Path
                },
                Metadata = new Dictionary<string, string>
                {
                    // { "AuthorizationPolicy", r.AuthorizationPolicy },
                    { "AuthorizationPolicy", "AdminPolicy" }
                },
                Transforms = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>{
                        { "PathPattern", r.Path },
                    }
                },
                RateLimiterPolicy = "fixed",
                AuthorizationPolicy = r.AuthorizationPolicy
            }).ToList();

            var clusterConfigs = clusters.GroupBy(c => c.ClusterId).Select(g => new ClusterConfig
            {
                ClusterId = g.Key,
                Destinations = g.ToDictionary(c => c.ClusterId + "Api", c => new DestinationConfig { Address = c.DestinationAddress }),
                HealthCheck = new HealthCheckConfig
                {
                    Active = new ActiveHealthCheckConfig
                    {
                        Enabled = true,
                        Interval = TimeSpan.FromSeconds(10),
                        Timeout = TimeSpan.FromSeconds(3),
                        Path = "/health",
                        Policy = HealthCheckConstants.ActivePolicy.ConsecutiveFailures
                    },
                    Passive = new PassiveHealthCheckConfig
                    {
                        Enabled = true,
                        Policy = HealthCheckConstants.PassivePolicy.TransportFailureRate,
                        ReactivationPeriod = TimeSpan.FromMinutes(5)
                    }
                }
            }).ToList();

            var config = new CustomProxyConfig(routeConfigs, clusterConfigs);

            await redis.AddOrUpdate(cacheKey, JsonConvert.SerializeObject(config));//_cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(config));
            logger.LogInformation("Configuration loaded and cached.");
            return config;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return _config ?? new CustomProxyConfig(new List<RouteConfig>(), new List<ClusterConfig>());

        }
    }

    public void ReloadConfig()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var redis = scope.ServiceProvider.GetRequiredService<IRedisRepository>();
        var cacheKey = "YarpProxyConfig";
        redis.Delete(cacheKey);
        _config = LoadConfig().Result;
        var oldToken = _changeToken;
        _changeToken = new CancellationChangeToken(new CancellationToken(false));
        // oldToken.Cancel();
    }

    private class CustomProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters) : IProxyConfig
    {
        public IReadOnlyList<RouteConfig> Routes { get; } = routes;

        public IReadOnlyList<ClusterConfig> Clusters { get; } = clusters;

        [JsonIgnore]
        public IChangeToken ChangeToken { get; private set; } = new CancellationChangeToken(new CancellationToken(false));
    }
}


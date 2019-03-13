using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace WM.Common.Healthchecks
{
    public class RedisServerHealthCheck : IHealthCheck
    {
        private readonly IDistributedCache _distributedCache;

        public RedisServerHealthCheck(IConfiguration configuration, IDistributedCache distributedCache) //SqlConnection connection)
        {
            _distributedCache = distributedCache;
        }

        public string Name => "Redis Health Check";

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await _distributedCache.SetStringAsync("Healthcheck", "true", cancellationToken);
                if (await _distributedCache.GetStringAsync("Healthcheck", cancellationToken) =="true")
                    return HealthCheckResult.Healthy();

            }
            catch (SqlException)
            {
                return HealthCheckResult.Unhealthy();
            }

            return HealthCheckResult.Healthy();
        }
    }
}


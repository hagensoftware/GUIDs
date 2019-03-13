using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace WM.Common.Healthchecks
{
    public class SqlServerHealthCheck : IHealthCheck
    {
        SqlConnection _connection;

        public SqlServerHealthCheck(IConfiguration configuration) 
        {
            _connection = new SqlConnection(configuration["connectionStrings:database"]);

            //_connection = //connection;
        }

        public string Name => "SQL Health Check";

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                if (_connection != null && _connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
            }
            catch (SqlException)
            {
                return HealthCheckResult.Unhealthy();
            }

            return HealthCheckResult.Healthy();
        }
    }
}


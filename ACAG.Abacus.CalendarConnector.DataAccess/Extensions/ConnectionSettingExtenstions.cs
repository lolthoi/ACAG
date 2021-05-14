using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Extensions
{
    public static class ConnectionSettingExtenstions
    {
        public static IServiceCollection ReadConnectionSettingConfig(this IServiceCollection services, IConfiguration configuration, params string[] connectionNames)
        {
            services.Configure<ConnectionSettings>(options =>
            {
                configuration.GetSection(nameof(ConnectionSettings)).Bind(options);
                if (connectionNames != null && connectionNames.Length > 0)
                {
                    options.ConnectionStrings = new Dictionary<string, string>();

                    foreach (var connStr in connectionNames)
                    {
                        var connValue = configuration.GetConnectionString(connStr);
                        options.ConnectionStrings[connStr] = connValue;
                    }
                }
            });

            return services;
        }
    }
}

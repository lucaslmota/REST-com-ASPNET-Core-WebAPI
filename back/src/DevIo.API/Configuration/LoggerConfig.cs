using Elmah.Io.Extensions.Logging;

namespace DevIo.API.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfigurationn(this IServiceCollection services)
        {
            services.AddElmahIo(o =>
            {
                o.ApiKey = "aa249f42e2344c99bf6b1fa1a4e872d4";
                o.LogId = new Guid("e985cdb5-0c6a-4d16-a3ad-77ca07d1c1ad");
            });

            services.AddLogging(builder =>
            {
                builder.AddElmahIo(o =>
                {
                    o.ApiKey = "aa249f42e2344c99bf6b1fa1a4e872d4";
                    o.LogId = new Guid("e985cdb5-0c6a-4d16-a3ad-77ca07d1c1ad");
                });
                builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);
            });

            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();

            return app;
        }
    }
}

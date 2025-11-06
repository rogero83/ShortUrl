using ShortUrl.WebApp.BackgroungService;

namespace ShortUrl.WebApp.Integrations
{
    public static class IntegrationsWebService
    {
        public static IHostApplicationBuilder AddIntegrationsWebService(this IHostApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddHostedService<ClickEventProcessorService>();

            builder.Services.AddProblemDetails();

            return builder;
        }
    }
}

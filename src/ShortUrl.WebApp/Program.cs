using ShortUrl.Infrastructure;
using ShortUrl.WebApp.EndPoints;
using ShortUrl.WebApp.Integrations;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.AddInfrastructureService();
builder.AddIntegrationsWebService(builder.Configuration);

var app = builder.Build();
app.UseStaticFiles();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Simple ShortUrl Project");

app.MapShortUrlEndpoints()
    .MapManageShortUrlEndpoints()
    .MapQrCodeEndpoints();

await app.RunAsync();

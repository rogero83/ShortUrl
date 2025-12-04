using Microsoft.Extensions.DependencyInjection;
using ShortUrl.AppHost.Commands;

var builder = DistributedApplication.CreateBuilder(args);

var isTest = args.Length > 0 && args.Contains("test");

var pgsql = builder.AddPostgres("pgsql")
        .WithHostPort(15432);
if (!isTest)
{
    pgsql.WithDataVolume("short-url-mydb")
        .WithLifetime(ContainerLifetime.Persistent);
}

var db = pgsql.AddDatabase("short-url-db");
db.WithRestoreDbCommand();

var redis = builder.AddRedis("redis")
    .WithHostPort(16379);
if (!isTest)
{
    redis.WithClearCommand()
    .WithDataVolume("short-url-redis")
    .WithLifetime(ContainerLifetime.Persistent);
}

var app = builder.AddProject<Projects.ShortUrl_WebApp>("shorturl-webapp")
    .WithExternalHttpEndpoints()
    //.WithReplicas(3)    
    .WithReference(db).WaitFor(pgsql)
    .WithReference(redis).WaitFor(redis)
    .WithUrls(context =>
    {
        foreach (var url in context.Urls)
        {
            url.DisplayLocation = UrlDisplayLocation.DetailsOnly;
        }

        context.Urls.Add(new ResourceUrlAnnotation
        {
            Url = "/swagger",
            DisplayText = "Swagger Doc",
            Endpoint = context.GetEndpoint("https")
        });

        context.Urls.Add(new ResourceUrlAnnotation
        {
            Url = "/scalar",
            DisplayText = "Scalar Doc",
            Endpoint = context.GetEndpoint("https")
        });
    })
    ;

// Migrate database
var efmigrate = builder.AddEfMigrate(app, db);
app.WaitForCompletion(efmigrate);
app.WithChildRelationship(efmigrate);

// Add seed command
app.WithDataPopulation();

await builder.Build().RunAsync();
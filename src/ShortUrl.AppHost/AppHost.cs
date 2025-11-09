using ShortUrl.AppHost;
using ShortUrl.AppHost.Commands;

var builder = DistributedApplication.CreateBuilder(args);

var pgsql = builder.AddPostgres("pgsql")
    .WithHostPort(15432)
    .WithDataVolume("short-url-mydb")
    .WithLifetime(ContainerLifetime.Persistent);

var db = pgsql.AddDatabase("short-url-db");

var redis = builder.AddRedis("redis")
    .WithHostPort(16379)
    .WithClearCommand()
    .WithDataVolume("short-url-redis")
    .WithLifetime(ContainerLifetime.Persistent);

var consoleApp = builder.AddProject<Projects.ShortUrl_DevSupport>("shorturl-devsupport")
    .WithSeedCommand() // <-- Non funziona aspettare nuova versione
    .WithArgs(
    "migrate",
    "seed"
    ) // <-- Funziona da usare in attesa di nuova versione
    .WithReference(db).WaitFor(pgsql)
    .WithAnnotation(new CustomTagAnnotation("console"));

builder.AddProject<Projects.ShortUrl_WebApp>("shorturl-webapp")
    .WithExternalHttpEndpoints()
    //.WithReplicas(3)    
    .WithReference(db).WaitFor(pgsql)
    .WithReference(redis).WaitFor(redis)
    .WaitFor(consoleApp);



await builder.Build().RunAsync();

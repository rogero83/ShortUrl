using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Net.Http.Json;

namespace ShortUrl.AppHost.Commands;

internal static class ExtMethods
{
    extension(IResourceBuilder<RedisResource> builder)
    {
        public IResourceBuilder<RedisResource> WithClearCommand()
        {
            return builder.WithCommand("clear-cache", "Clear Cache",
                context => OnRunClearCacheCommandAsync(builder, context));

            static async Task<ExecuteCommandResult> OnRunClearCacheCommandAsync(
                IResourceBuilder<RedisResource> builder,
                ExecuteCommandContext context)
            {
                var connectionString = await builder.Resource.GetConnectionStringAsync()
                    ?? throw new InvalidOperationException($"Cannot get connection string for '{context.ResourceName}'.");

                await using var connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
                var database = connection.GetDatabase();
                await database.ExecuteAsync("FLUSHALL");

                return CommandResults.Success();
            }
        }
    }

    extension(IDistributedApplicationBuilder builder)
    {
        public IResourceBuilder<ExecutableResource> AddEfMigrate(IResourceBuilder<ProjectResource> app,
            IResourceBuilder<IResourceWithConnectionString> database)
        {
            var projectDirectory = Path.GetDirectoryName(app.Resource.GetProjectMetadata().ProjectPath)!;

            var efmigrate = builder.AddExecutable($"ef-migrate-{app.Resource.Name}",
                    "dotnet", projectDirectory)
                .WithArgs("ef", "database", "update",
                "--project", "..\\ShortUrl.DbPersistence",
                "--startup-project", "..\\ShortUrl.DevSupport",
                "--connection", database.Resource)

                .WithEnvironment("DOTNET_ENVIRONMENT", "Development")
                .WithReference(database)
                .WaitFor(database);

            return efmigrate;
        }
    }

    extension(IResourceBuilder<ProjectResource> resourceBuilder)
    {
        public IResourceBuilder<ProjectResource> WithDataPopulation()
        {
            return resourceBuilder.WithCommand("seed-data", "Seed the database", (context) =>
            {
                return SeedDatabaseAsync(resourceBuilder, context);
            });

            static async Task<ExecuteCommandResult> SeedDatabaseAsync(IResourceBuilder<ProjectResource> app,
                ExecuteCommandContext context)
            {
                var cancellationToken = context.CancellationToken;

                var interactionService = context.ServiceProvider.GetRequiredService<IInteractionService>();

                var interactionApiKey = new InteractionInput
                {
                    Name = "API Key",
                    InputType = InputType.Choice,
                    Label = "Select the API key to use for data insertion:",
                    Options = new[]
                    {
                        new KeyValuePair<string, string>("api-key-local","Local API Key" ),
                        new KeyValuePair<string, string>("api-key-local-custom-url","Local API Key with Custom URL Permission")
                    },
                };

                var interactionNumberOfItem = new InteractionInput
                {
                    Name = "Number of Items",
                    InputType = InputType.Number,
                    Label = "Enter the number of items to create:",
                    Value = "100"
                };

                var confirmed = await interactionService.PromptInputsAsync("Insert Fake data",
                    "Select Develop api-ky and number of item",
                    [interactionApiKey, interactionNumberOfItem], cancellationToken: cancellationToken);

                if (confirmed.Data is null || confirmed.Canceled)
                {
                    return new ExecuteCommandResult
                    {
                        Success = false,
                        ErrorMessage = "Operation cancelled by user."
                    };
                }

                var logger = context.ServiceProvider.GetRequiredService<ResourceLoggerService>()
                    .GetLogger(app.Resource)!;

                logger.LogInformation("Starting database seeding...");

                using var httpClient = new HttpClient();
                var endPoint = app.Resource.GetEndpoint("https");
                var baseUrl = await endPoint.GetValueAsync(cancellationToken);

                var faker = new Faker();

                httpClient.DefaultRequestHeaders.Add("x-apikey", interactionApiKey.Value);
                for (int i = 1; i <= int.Parse(interactionNumberOfItem.Value); i++)
                {
                    var request = new CreateShortUrlRequest(
                        OriginalUrl: faker.Internet.Url(),
                        Activate: faker.Random.Bool(),
                        Expire: faker.Date.Future().ToUniversalTime()
                    );

                    var response = await httpClient.PostAsJsonAsync(
                        $"{baseUrl}/api/v1/create",
                        request,
                        cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        logger.LogInformation("Created short URL {Index}/50", i);
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync(cancellationToken);
                        logger.LogError("Failed to create short URL {Index}/50. Status: {Status}. Response: {Response}",
                            i, response.StatusCode, content);
                    }
                }

                return new ExecuteCommandResult { Success = true };
            }
        }
    }

    extension(IResourceBuilder<PostgresDatabaseResource> resourceBuilder)
    {
        public IResourceBuilder<PostgresDatabaseResource> WithRestoreDbCommand()
        {
            return resourceBuilder.WithCommand("reset", "Reset Database", async context =>
            {
                var cancellationToken = context.CancellationToken;

                var interactionService = context.ServiceProvider.GetRequiredService<IInteractionService>();
                var confirmed = await interactionService.PromptConfirmationAsync(
                    $"Are you sure you want to reset the database '{resourceBuilder.Resource.Name}'?",
                    "This action cannot be undone.");

                if (!confirmed.Data || confirmed.Canceled)
                {
                    return new ExecuteCommandResult
                    {
                        Success = false,
                        ErrorMessage = "Operation cancelled by user."
                    };
                }

                var rcs = context.ServiceProvider.GetRequiredService<ResourceCommandService>();

                await rcs.ExecuteCommandAsync(
                    resourceBuilder.Resource.Parent,
                    KnownResourceCommands.RestartCommand,
                    cancellationToken);

                return new ExecuteCommandResult { Success = true };
            });
        }
    }
}

// Code from: src/ShortUrl.WebApp/EndPoints/ManageShortUrlEndpoints.cs
internal record CreateShortUrlRequest(string OriginalUrl,
        bool Activate = true,
        DateTime? Expire = null)
{
    /// <summary>
    /// Only for apikeys with permission to set custom short codes.
    /// </summary>
    public string? ShortCode { get; set; }
};

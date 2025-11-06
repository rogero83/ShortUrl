using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ShortUrl.AppHost.Commands
{
    internal static class RedisResourceBuilderExtensions
    {
        public static IResourceBuilder<RedisResource> WithClearCommand(
            this IResourceBuilder<RedisResource> builder)
        {
            var commandOptions = new CommandOptions
            {
                IconName = "AnimalRabbitOff",
                IconVariant = IconVariant.Filled,
                UpdateState = OnUpdateResourceState
            };

            builder.WithCommand(
                name: "clear-cache",
                displayName: "Clear Cache",
                executeCommand: context => OnRunClearCacheCommandAsync(builder, context),
                commandOptions: commandOptions);

            return builder;
        }

        private static async Task<ExecuteCommandResult> OnRunClearCacheCommandAsync(
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

        private static ResourceCommandState OnUpdateResourceState(UpdateCommandStateContext context)
        {
            var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Resource snapshot: {Snapshot}", context.ResourceSnapshot);

            return context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
                ? ResourceCommandState.Enabled
                : ResourceCommandState.Disabled;
        }
    }
}

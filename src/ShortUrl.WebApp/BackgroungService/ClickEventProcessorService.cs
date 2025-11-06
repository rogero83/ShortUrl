using ShortUrl.Core.Contracts;
using ShortUrl.Core.Models;
using System.Diagnostics;

namespace ShortUrl.WebApp.BackgroungService
{
    public class ClickEventProcessorService(IClickEventChannel _messageChannel,
        IServiceProvider serviceProvider,
        ActivitySource activitySource,
        ILogger<ClickEventProcessorService> logger)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("ClickEventProcessorService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _messageChannel.ReadAsync(stoppingToken);
                try
                {
                    // Create new activity for processing the click event
                    using var activity = activitySource.StartActivity(
                        "ClickEventProcessorService",
                        ActivityKind.Internal,
                        message.Activity?.Context ?? default);

                    // Process the click event message
                    await ProcessClickEventAsync(message, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing ClickEvent: {ClickEvent}", message);
                }
            }
        }

        private async Task ProcessClickEventAsync(ClickEventItem message, CancellationToken stoppingToken)
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var clickEventService = scope.ServiceProvider.GetRequiredService<IClickEventService>();
            await clickEventService.LogClickEventAsync(message, stoppingToken);
        }
    }
}

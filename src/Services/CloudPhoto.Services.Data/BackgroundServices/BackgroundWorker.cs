namespace CloudPhoto.Services.Data.BackgroundServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using BackgroundQueue;
    using ImageHelper;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class BackgroundWorker : BackgroundService
    {
        private readonly IBackgroundQueue<ImageInfoParams> queue;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<BackgroundWorker> logger;

        public BackgroundWorker(
            IBackgroundQueue<ImageInfoParams> queue,
            IServiceScopeFactory scopeFactory,
            ILogger<BackgroundWorker> logger)
        {
            this.queue = queue;
            this.scopeFactory = scopeFactory;
            this.logger = logger;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogCritical(
                "The {Type} is stopping due to a host shutdown, queued items might not be processed anymore.",
                nameof(BackgroundWorker));

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("{Type} is now running in the background.", nameof(BackgroundWorker));

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(500, stoppingToken);
                    var book = await queue.Dequeue();

                    if (book == null)
                    {
                        continue;
                    }

                    logger.LogInformation("Book found! Starting to process ..");

                    using var scope = scopeFactory.CreateScope();
                    var publisher = scope.ServiceProvider.GetRequiredService<IImageHelper>();

                    await publisher.UploadImage(book, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogCritical("An error occurred when publishing a book. Exception: {@Exception}", ex);
                }
            }
        }
    }
}

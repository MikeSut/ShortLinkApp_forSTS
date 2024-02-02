namespace ShortLinks.Presentation.Api.Telegram;

public class TelegramBotPollingService(
    IServiceProvider services,
    ILogger<TelegramBotPollingService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            try
            {
                using var updateServiceScope = services.CreateScope();

                var updateReceiver = updateServiceScope.ServiceProvider
                    .GetRequiredService<TelegramUpdateReceiveService>();
            
                logger.LogDebug("Polling next telegram update");

                await updateReceiver.ReceiveAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            
        }
    }
}
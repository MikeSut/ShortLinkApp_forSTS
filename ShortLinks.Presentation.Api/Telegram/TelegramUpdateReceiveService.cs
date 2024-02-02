using Telegram.Bot;

namespace ShortLinks.Presentation.Api.Telegram;

public class TelegramUpdateReceiveService(
    ITelegramBotClient telegramBotClient, TelegramUpdateHandler telegramUpdateHandler)
{
    public Task ReceiveAsync(CancellationToken cancellationToken)
    {
        return telegramBotClient.ReceiveAsync(telegramUpdateHandler, cancellationToken: cancellationToken);
    }
}
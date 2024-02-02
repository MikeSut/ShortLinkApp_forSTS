namespace ShortLinks.Presentation.Api.Telegram;

public sealed class TelegramBotOptions
{
    public const string ConfigurationSectionPath = "Services: BotToken";
    public required string Token { get; init; } = "6303027654:AAGDrnppv5c0PnKU9IS5qVY6C1uEwGGFteg";
}
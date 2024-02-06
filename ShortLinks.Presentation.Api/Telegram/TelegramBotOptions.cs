namespace ShortLinks.Presentation.Api.Telegram;

public sealed class TelegramBotOptions
{
    public const string ConfigurationSectionPath = "Services: BotToken";
    public required string Token { get; init; } = "";
}
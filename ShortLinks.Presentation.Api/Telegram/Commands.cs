using PRTelegramBot.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShortLinks.Presentation.Api.TelegramCommands;

public class Commands
{
    [ReplyMenuHandler("/start")]
    public static async Task Example(ITelegramBotClient botClient, Update update)
    {
        var message = "Введите логин:";
        var sendMessage = await PRTelegramBot.Helpers.Message.Send(botClient, update, message);
        
        
    }
}
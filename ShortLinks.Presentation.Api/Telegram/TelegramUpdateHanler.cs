using ShortLinks.Application;
using Microsoft.Extensions.Logging;
using ShortLinks.Domain.Entity;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ShortLinks.Presentation.Api.Telegram;

public class TelegramUpdateHandler(ILogger<IUpdateHandler> logger, ApplicationDbContext? db) : IUpdateHandler
{
    private const string Button1 = "Get all ShortLinks";
    private const string Button2 = "Get only Permanent ShortLinks";
    
    public async Task HandleUpdateAsync(
        ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope("Handle Telegram Update: {UpdateId}", update.Id);
        
        var handleTask = update switch
        {
            { Message.Text: { } messageText } when messageText.StartsWith("/")
                => HandleCommand(client, update, messageText, cancellationToken),
            _
                => HandleDefault(client, update, cancellationToken)
        };
        await handleTask;
    }

    private async Task HandleCommand(
        ITelegramBotClient client, Update update, string messageText, CancellationToken cancellationToken)
    {
        if (messageText.ToLower().Contains("/start "))
        {
            var message = update.Message;
            if (db?.TgChatIdUsers.FirstOrDefault(x => x.ChatId == message.Chat.Id) == null)
            {
                var isExistUser = await isExistsUserTask(messageText, message.Chat.Id);
                if (isExistUser == false)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Вы не зарегистрированы", cancellationToken: cancellationToken);
                    return;
                }
            }
        }
        

        


    }

    private IReplyMarkup? GetButtons()
    {
        return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>()) 
        {
            Keyboard = new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Button1) },
                new List<KeyboardButton>{ new KeyboardButton(Button2) }

            }

        };
    }


    public Task HandlePollingErrorAsync(
        ITelegramBotClient client, Exception exception, CancellationToken cancellationToken
    ) {
        logger.LogError(exception, "Caught unexpected exception during telegram update handle");

        return Task.CompletedTask;
    }
    
    private Task HandleDefault(
        ITelegramBotClient client, Update update, CancellationToken cancellationToken
    ) {
        logger.LogDebug("Received update has not been handled: {@Update}", update);

        return Task.CompletedTask;
    }

    private Task<bool> isExistsUserTask(string messageText, long chatId)
    {
        var userName = messageText.Split(' ')[1];
        var user = db?.Users.FirstOrDefault(x => x.UserName == userName);

        if (user == null)
        {
            return Task.FromResult(false);
        }
        
        var userChatId = new TgChatId()
        {
            UserId = user.Id,
            ChatId = chatId
        };
        db?.TgChatIdUsers.Add(userChatId);
        db?.SaveChangesAsync();
        return Task.FromResult(true);

        
    }


    
}
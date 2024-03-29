using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Http.HttpResults;
using ShortLinks.Application;
using ShortLinks.Domain.Entity;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Options;
using QRCoder;
using InputFile = Microsoft.AspNetCore.Components.Forms.InputFile;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;


namespace ShortLinks.Presentation.Api.Telegram;

public class TelegramUpdateHandler(ILogger<IUpdateHandler> logger, ApplicationDbContext? db, 
    IOptions<CredentialsOptions> optProvider) : IUpdateHandler
{
    private readonly string _publicAddress = optProvider.Value.PublicAddress;
    private const string Button1 = "Get all ShortLinks";
    private const string Button2 = "Get only Permanent ShortLinks";
    private const string Button3 = "Generate QR-Code";
    private const string Button4 = "Restart the bot";


    private static readonly string[] Buttons = [Button1, Button2, Button3, Button4];


    public async Task HandleUpdateAsync(
        ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope("Handle Telegram Update: {UpdateId}", update.Id);
        
        var handleTask = update switch
        {
            { Message.Text: { } messageText } when messageText.StartsWith("/")
                => HandleCommand(client, update, messageText, cancellationToken),
            {Message.Text: { } messageText } when Buttons.Any(x => x == messageText)
                => HandleButton(client, update, messageText, cancellationToken),
            _
                => HandleDefault(client, update, cancellationToken)
        };
        await handleTask;
    }

    private Task HandleCommand(
        ITelegramBotClient client, Update update, string messageText, CancellationToken cancellationToken)
    {
        var chatId = update.Message!.Chat.Id;
        if (messageText.ToLower().Contains("/start "))
        {
            var userName = messageText.Split(' ')[1];
            var user = db?.Users.FirstOrDefault(x => x.UserName == userName);
            if (user is null)
            {
                return client.SendTextMessageAsync(
                    chatId, "Пользователь с таким UserName не найден", cancellationToken: cancellationToken
                );
            }

            SaveChatId(user.Id, chatId);
            return client.SendTextMessageAsync(
                chatId, $"Добрый день, {user.Name}. Добро пожаловать в сервис коротких ссылок! " +
                        $"Выберите дальнейшую комманду:", replyMarkup: GetButtons(), cancellationToken: cancellationToken
            );
            
        }

       return messageText switch {
            _
                => HandleDefault(client, update, cancellationToken)
        };
    }


    private Task HandleButton(ITelegramBotClient client, Update update, string messageText,
        CancellationToken cancellationToken)
    {
        var chatId = update.Message!.Chat.Id;

        return messageText switch
        {
            Button1
                => GetAllLinks(chatId, client, update, cancellationToken),
            Button2
                => GetOnlyPermanentLinks(chatId, client, update, cancellationToken),
            Button3
                => GetQrCode(chatId, client, update, cancellationToken),
            Button4
                => BotRestart(chatId, client, update, cancellationToken),
            _
                => HandleDefault(client, update, cancellationToken)
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

    
    private Task GetOnlyPermanentLinks(long chatId, ITelegramBotClient client, Update update, 
        CancellationToken cancellationToken)
    {
        var allLinks = "";
        var userId = db?.TgChatIdUsers.FirstOrDefault(x => x.ChatId == chatId)?.UserId;
        var allLinksUsers = db?.Urls.Where(x => 
            x.UserId == userId && x.Permanent == "yes");
        if (allLinksUsers == null) 
        {
            return client.SendTextMessageAsync(
                chatId, $"У вас еще нет перманентных коротких ссылок.", cancellationToken: cancellationToken);
        }

        foreach (var z in allLinksUsers)
        {
            allLinks +=  _publicAddress + "/" + z.ShortUrl + "\n";
        }

        return client.SendTextMessageAsync(
            chatId, $"{allLinks} \nЕсли вы хотите сгенерировать qr-code для вашей ссылки, " +
                    $"вставьте соответствующую ссылку в строку и выберите команду 'Generate QR-Code'.", replyMarkup: GetButtonQr(), cancellationToken: cancellationToken
        );    
    }

    private Task GetAllLinks(long chatId, ITelegramBotClient client, Update update,
        CancellationToken cancellationToken)
    {
        var allLinks = "";
        var userId = db?.TgChatIdUsers.FirstOrDefault(x => x.ChatId == chatId)?.UserId;
        var allLinksUsers = db?.Urls.Where(x => 
            x.UserId == userId && x.ExpirationDate >= DateTime.UtcNow || x.UserId == userId && x.Permanent == "yes");
        if (allLinksUsers == null) 
        {
            return client.SendTextMessageAsync(
                chatId, $"У вас еще нет сгенерированных коротких ссылок.", cancellationToken: cancellationToken);
        }

        foreach (var z in allLinksUsers)
        {
            allLinks +=  _publicAddress + "/" + z.ShortUrl + "\n";
        }

        return client.SendTextMessageAsync(
            chatId, $"{allLinks} \nЕсли вы хотите сгенерировать qr-code для вашей ссылки, " +
                    $"вставьте соответствующую ссылку в строку и выберите команду 'Generate QR-Code'.", replyMarkup: GetButtonQr(), cancellationToken: cancellationToken
        );
    }

    private Task GetQrCode(long chatId, ITelegramBotClient client, Update update,
        CancellationToken cancellationToken)
    {
        var link = update.Message?.Text;
        QRCodeGenerator qrCode = new QRCodeGenerator();
        QRCodeData data = qrCode.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
        PngByteQRCode qRCode = new PngByteQRCode(data);
        byte[] qrCodeBytes = qRCode.GetGraphic(20);
        var img = Image.Load(qrCodeBytes);
        using MemoryStream ms = new MemoryStream();
        img.SaveAsPng(ms);
        
        ms.Position = 0;
        return client.SendPhotoAsync(
            chatId, new InputFileStream(ms), cancellationToken: cancellationToken);
    }
    
    
    
    private Task BotRestart(long chatId, ITelegramBotClient client, Update update,
        CancellationToken cancellationToken)
    {
        var userId = db?.TgChatIdUsers.FirstOrDefault(x => x.ChatId == chatId)?.UserId;
        var user = db?.Users.FirstOrDefault(x => x.Id == userId);
        

        return client.SendTextMessageAsync(
            chatId, $"Добрый день, {user.Name}. Добро пожаловать в сервис коротких ссылок! Выберите дальнейшую комманду:", replyMarkup: GetButtons(), cancellationToken: cancellationToken
        );
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
    
    private IReplyMarkup? GetButtonQr()
    {
        return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>()) 
        {
            Keyboard = new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Button3) },
                new List<KeyboardButton>{ new KeyboardButton(Button4) },

            }
        };
    }
    
    private IReplyMarkup? GetButtonRestartBot()
    {
        return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>()) 
        {
            Keyboard = new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Button4) },
            }
        };
    }
    private async Task SaveChatId(int userId, long chatId)
    {
        var getUser = db?.TgChatIdUsers.FirstOrDefault(x => x.ChatId == chatId);
        if (getUser == null)
        {
            var newTgChatId = new TgChatId()
            {
                UserId = userId,
                ChatId = chatId
            };
            db?.TgChatIdUsers.Add(newTgChatId);
            await db.SaveChangesAsync();
        }
    }

}
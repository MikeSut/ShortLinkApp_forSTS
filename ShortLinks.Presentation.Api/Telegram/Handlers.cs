using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShortLinks.Presentation.Api.Telegram;

public static class Handlers
{
    // public static ApplicationDbContext db;
    public static async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
   {
       var message = update.Message;
       Console.WriteLine($"{message?.Chat.FirstName}  |  {message?.Text}");
       if (message?.Text != null)
       { 
           if (message.Text.Contains("Привет", StringComparison.CurrentCultureIgnoreCase))
           {
               await botClient.SendTextMessageAsync(message.Chat.Id, "Введите UserName:", cancellationToken: token);
               return;
       //         var userName = db.Users.FirstOrDefault(x => x.UserName == message.Text);
       //         if (userName == null)
       //         {
       //             await botClient.SendTextMessageAsync(message.Chat.Id, $"Пользователь {message.Text} не найден");
       //         }
       //         await botClient.SendTextMessageAsync(message.Chat.Id, "Введите Password:");
           }
       }
       
   }

   public static async Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
   {
       throw new NotImplementedException();
   }
   
   
}

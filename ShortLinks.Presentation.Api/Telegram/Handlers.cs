using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShortLinks.Presentation.Api.TelegramCommands;

public static class Handlers
{
    public readonly struct Message
    {
        [Required, JsonInclude, JsonPropertyName("chat_id")]
        public readonly string ChatId;
       
        [Required, Range(1, 4096), JsonInclude, JsonPropertyName("text")]
        public readonly string Text;
 
        public Message(string chatId, string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length < 1 || text.Length > 4096)
            {
                throw new ArgumentOutOfRangeException(nameof(text), "Размер сообщения должен удовлетворять диапазону 1 - 4096");
            }
            
            ChatId = chatId;
            Text = text;
        }
    }
    
    public static async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
   {
       var message = update.Message;
       Console.WriteLine($"{message.Chat.FirstName}  |  {message.Text}");
       if (message.Text != null)
       {
           if (message.Text.ToLower().Contains("здорова"))
           {
               await botClient.SendTextMessageAsync(message.Chat.Id, "Изенер");
               
           }
       }
   }

   public static async Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
   {
       throw new NotImplementedException();
   }
}

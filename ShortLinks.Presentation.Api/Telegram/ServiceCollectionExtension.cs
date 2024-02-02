using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace ShortLinks.Presentation.Api.Telegram;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddTelegramBotServices(this IServiceCollection services)
    {
        services.AddOptions<TelegramBotOptions>()
            .BindConfiguration(TelegramBotOptions.ConfigurationSectionPath);
        
        services.AddHttpClient("ShortLink_bot")
        .AddTypedClient<ITelegramBotClient>(static (httpClient, services) =>
        {
            var options = services.GetRequiredService<IOptions<TelegramBotOptions>>().Value;
        
            return new TelegramBotClient(options: new (options.Token), httpClient);
        });

        services.AddTransient<TelegramUpdateReceiveService>();

        services.AddTransient<TelegramUpdateHandler>();

        services.AddHostedService<TelegramBotPollingService>();

        return services;
    }
}
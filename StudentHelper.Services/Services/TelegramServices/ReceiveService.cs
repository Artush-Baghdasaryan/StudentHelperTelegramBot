using Microsoft.Extensions.Hosting;
using StudentHelper.Services.Interfaces;
using Telegram.Bot.Types.Enums;

namespace StudentHelper.Services.Services.TelegramServices;
using Telegram.Bot;
using Telegram.Bot.Polling;

public class ReceiverService : BackgroundService
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IUpdateHandler _updateHandler;
    
    public ReceiverService(ITelegramBotClient telegramBotClient, IUpdateHandler updateHandler)
    {
        _telegramBotClient = telegramBotClient;
        _updateHandler = updateHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[]
            {
                UpdateType.Message,
                UpdateType.CallbackQuery
            },
            ThrowPendingUpdates = true,
        };
        
        var me = await _telegramBotClient.GetMeAsync(cancellationToken);
        Console.WriteLine($"Bot receiver updates for {me.Username}");
        
        await _telegramBotClient.ReceiveAsync(_updateHandler, receiverOptions, cancellationToken);
    }
}
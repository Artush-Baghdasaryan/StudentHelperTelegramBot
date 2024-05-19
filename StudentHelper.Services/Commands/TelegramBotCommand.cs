using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentHelper.Services.Commands;

public abstract class TelegramBotCommand
{
    protected readonly ITelegramBotClient BotClient;
    
    protected TelegramBotCommand(ITelegramBotClient botClient)
    {
        BotClient = botClient;
    }

    public abstract Task ExecuteCommand(Message message, CancellationToken cancellationToken);
}
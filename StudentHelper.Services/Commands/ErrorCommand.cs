using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentHelper.Services.Commands;

public class ErrorCommand : TelegramBotCommand
{
    public ErrorCommand(ITelegramBotClient botClient) : base(botClient)
    {
    }

    public override async Task ExecuteCommand(Message message, CancellationToken cancellationToken)
    {
        await BotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Что то пошло не тек. Повторите попытку еще раз!",
            cancellationToken: cancellationToken);
    }
}
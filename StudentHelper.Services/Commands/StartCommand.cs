using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentHelper.Services.Commands;

public class StartCommand : TelegramBotCommand
{
    public StartCommand(ITelegramBotClient botClient) : base(botClient)
    {
        
    }

    public override async Task ExecuteCommand(Message message, CancellationToken cancellationToken)
    {
        await BotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Привет, я бот по помощи студентам studenthelper!\n" +
                  "Я помогу тебе с решением любой задачи!\n" +
                  "Напиши тему для теста!",
            cancellationToken: cancellationToken);
    }
}
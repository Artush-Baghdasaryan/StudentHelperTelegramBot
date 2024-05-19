using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace StudentHelper.Services.Commands;

public class InvalidCommand : TelegramBotCommand
{
    public InvalidCommand(ITelegramBotClient botClient) : base(botClient)
    {

    }

    public override async Task ExecuteCommand(Message message, CancellationToken cancellationToken)
    {
        await BotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Введите цифру",
            cancellationToken: cancellationToken);
    }
}
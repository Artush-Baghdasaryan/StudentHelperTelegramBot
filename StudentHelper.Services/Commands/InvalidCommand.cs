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
    private readonly string _message;
    
    public InvalidCommand(ITelegramBotClient botClient, string message) : base(botClient)
    {
        _message = message;
    }

    public override async Task ExecuteCommand(Message message, CancellationToken cancellationToken)
    {
        await BotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: _message,
            cancellationToken: cancellationToken);
    }
}
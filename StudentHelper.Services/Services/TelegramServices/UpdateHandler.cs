using System.Text;
using Microsoft.Extensions.Options;
using StudentHelper.Services.Commands;
using StudentHelper.Services.Data;
using StudentHelper.Services.Interfaces;
using StudentHelper.Services.Models;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace StudentHelper.Services.Services.TelegramServices;

public class UpdateHandler : IUpdateHandler
{
    private readonly CommandBuilder _commandBuilder;
    private readonly BotDataContext _dataContext;
    
    public UpdateHandler(BotDataContext dataContext, CommandBuilder commandBuilder)
    {
        _commandBuilder = commandBuilder;
        _dataContext = dataContext;
    }

    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        Task.Run(() => HandleUpdate(botClient, update, cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    private async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;
        if (message is null)
        {
            return;
        }
        
        if (message.Text is not {} text)
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Я понимаю только текстовые сообщения.");
            
            return;
        }
        
        Console.WriteLine($"Received a '{text}' message in chat.");

        try
        {
            var command = _commandBuilder.GetCommand(text, message.Chat.Id);
            await command.ExecuteCommand(message, cancellationToken);
        }
        catch (Exception)
        {
            var errorCommand = _commandBuilder.GetErrorCommand();
            await errorCommand.ExecuteCommand(message, cancellationToken);
            _dataContext.RemoveTest(message.Chat.Id);
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

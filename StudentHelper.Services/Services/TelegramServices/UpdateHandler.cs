using System.Text;
using Microsoft.Extensions.Options;
using StudentHelper.Services.Commands;
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
    private readonly IQuizService _quizService;
    private readonly CommandBuilder _commandBuilder;

    public UpdateHandler(IQuizService quizService, CommandBuilder commandBuilder)
    {
        _quizService = quizService;
        _commandBuilder = commandBuilder;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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

        var command = _commandBuilder.GetCommand(text, message.Chat.Id);
        await command.ExecuteCommand(message, cancellationToken);
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

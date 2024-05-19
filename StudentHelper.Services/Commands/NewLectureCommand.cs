using System.Text;
using StudentHelper.Services.Data;
using StudentHelper.Services.Interfaces;
using StudentHelper.Services.Services.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StudentHelper.Services.Commands;

public class NewLectureCommand : TelegramBotCommand
{
    private readonly IQuizService _quizService;
    private readonly BotDataContext _dataContext;
    
    public NewLectureCommand(
        ITelegramBotClient botClient,
        IQuizService quizService,
        BotDataContext dataContext) : base(botClient)
    {
        _quizService = quizService;
        _dataContext = dataContext;
    }

    public override async Task ExecuteCommand(Message message, CancellationToken cancellationToken)
    {
        if (message.Text is not { } text)
        {
            return;
        }

        var waitingMessage = await BotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "⏳ Подождите ...",
            cancellationToken: cancellationToken
        );
        
        var newQuiz = await _quizService.GenerateNewQuiz(text);
        if (newQuiz.Lecture is not { } lecture)
        {
            return;
        }

        _dataContext.AddTest(message.Chat.Id, newQuiz);
        
        var replyKeyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "Начать тест" }
        })
        {
            ResizeKeyboard = true
        };
        
        await BotClient.DeleteMessageAsync(message.Chat.Id, waitingMessage.MessageId, cancellationToken);

        await BotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: lecture,
            replyMarkup: replyKeyboard,
            cancellationToken: cancellationToken
        );
    }
}
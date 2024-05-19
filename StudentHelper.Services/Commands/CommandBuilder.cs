using System.Text.RegularExpressions;
using StudentHelper.Services.Data;
using StudentHelper.Services.Interfaces;
using StudentHelper.Services.Services.Data;
using Telegram.Bot;

namespace StudentHelper.Services.Commands;

public class CommandBuilder
{
    private readonly ITelegramBotClient _botClient;
    private readonly IQuizService _quizService;
    private readonly BotDataContext _dataContext;
    
    public CommandBuilder(
        ITelegramBotClient botClient,
        IQuizService quizService,
        BotDataContext dataContext)
    {
        _botClient = botClient;
        _quizService = quizService;
        _dataContext = dataContext;
    }

    public TelegramBotCommand GetCommand(string command, long chatId)
    {
        return command switch
        {
            "/start" => new StartCommand(_botClient),
            "Начать тест" => new TestCommand(_botClient, _dataContext),
            _ => GetCorrectCommand(command, chatId)
        };
    }

    private TelegramBotCommand GetCorrectCommand(string command, long chatId)
    {
        var test = _dataContext.GetTestByChatId(chatId);
        if (test is not null && test.TestStarted)
        {
            if (IsCommandValid(command))
            {
                // комманда которая отправляет предупрждение о том что текст не валидный
            }
            return new NextQuestionCommand(_botClient, _dataContext);
        }

        return new NewLectureCommand(_botClient, _quizService, _dataContext);
    }

    private bool IsCommandValid(string command)
    {
        Regex numericRegex = new Regex("^[0-9]+$");
        return numericRegex.IsMatch(command);
    }
}
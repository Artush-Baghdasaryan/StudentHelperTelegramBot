using System.Text.RegularExpressions;
using StudentHelper.Services.Data;
using StudentHelper.Services.Interfaces;
using StudentHelper.Services.Models;
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
        var chatState = _dataContext.GetTestByChatId(chatId)?.ChatState;
        
        return chatState switch
        {
            ChatState.None => GetBaseCommand(command),
            ChatState.Lecture => GetCommandDuringLecture(command),
            ChatState.Test => GetNextQuestionCommand(command),
            null => GetBaseCommand(command),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public TelegramBotCommand GetErrorCommand()
    {
        return new ErrorCommand(_botClient);
    }

    private TelegramBotCommand GetBaseCommand(string command)
    {
        return command switch
        {
            "/start" => new StartCommand(_botClient),
            _ => new NewLectureCommand(_botClient, _quizService, _dataContext)
        };
    }

    private TelegramBotCommand GetCommandDuringLecture(string command)
    {
        if (command.Equals("Начать тест"))
        {
            return new TestCommand(_botClient, _dataContext);
        }

        return new InvalidCommand(_botClient, "Только после окончания теста вы можете перейти к следующей лекции");
    }

    private TelegramBotCommand GetNextQuestionCommand(string command)
    {
        if (IsCommandValid(command))
        {
            return new NextQuestionCommand(_botClient, _dataContext);
        }

        return new InvalidCommand(_botClient, "Введите цифру!");
    }

    private bool IsCommandValid(string command)
    {
        Regex numericRegex = new Regex("^[0-9]+$");
        return numericRegex.IsMatch(command);
    }
}
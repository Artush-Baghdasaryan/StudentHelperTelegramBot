using StudentHelper.Services.Interfaces;
using Telegram.Bot;

namespace StudentHelper.Services.Commands;

public class CommandBuilder
{
    private readonly ITelegramBotClient _botClient;
    private readonly IQuizService _quizService;
    
    public CommandBuilder(
        ITelegramBotClient botClient,
        IQuizService quizService)
    {
        _botClient = botClient;
        _quizService = quizService;
    }

    public TelegramBotCommand GetCommand(string command)
    {
        return command switch
        {
            "/start" => new StartCommand(_botClient),
            _ => new NewLectureCommand(_botClient, _quizService)
        };
    }
}
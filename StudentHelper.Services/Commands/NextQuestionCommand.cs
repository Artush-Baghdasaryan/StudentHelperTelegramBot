using StudentHelper.Services.Data;
using StudentHelper.Services.Services.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentHelper.Services.Commands;

public class NextQuestionCommand : TestCommand
{
    private readonly BotDataContext _dataContext;
    
    public NextQuestionCommand(
        ITelegramBotClient botClient,
        BotDataContext dataContext) : base(botClient, dataContext)
    {
        _dataContext = dataContext;
    }

    public override async Task ExecuteCommand(Message message, CancellationToken cancellationToken)
    {
        await SetAnswerToQuestion(message.Chat.Id, message.Text!);
        UpdateCurrentQuestionIndex(message.Chat.Id);

        await base.ExecuteCommand(message, cancellationToken);
        
    }

    private Task SetAnswerToQuestion(long chatId, string answer)
    {
        var test = _dataContext.GetTestByChatId(chatId);
        if (test is null)
        {
            return Task.CompletedTask;
        }

        var currentQuestion = test.QuestionResults?[test.CurrentQuestionIndex];
        if (currentQuestion is null)
        {
            throw new InvalidOperationException();
        }
        
        currentQuestion.UserAnswer = answer;
        _dataContext.UpdateTest(chatId, test);
        return Task.CompletedTask;
    }
    
    private void UpdateCurrentQuestionIndex(long chatId)
    {
        var test = _dataContext.GetTestByChatId(chatId);
        if (test is null)
        {
            throw new InvalidOperationException();
        }
        
        test.CurrentQuestionIndex++;
        _dataContext.UpdateTest(chatId, test);
    }

    private int GetCurrentQuestionIndex(long chatId)
    {
        var test = _dataContext.GetTestByChatId(chatId);
        return test?.CurrentQuestionIndex ?? throw new InvalidOperationException();
    }
}
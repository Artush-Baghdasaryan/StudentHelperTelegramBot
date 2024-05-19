using System.Text;
using StudentHelper.Services.Data;
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
        var finished = await FinishTestIfNeed(message.Chat.Id, cancellationToken);
        if (finished)
        {
            return;
        }
        
        UpdateCurrentQuestionIndex(message.Chat.Id);

        await base.ExecuteCommand(message, cancellationToken);
    }

    private async Task<bool> FinishTestIfNeed(long chatId, CancellationToken cancellationToken)
    {
        var result = new StringBuilder();
        var correctAnswerEmoji = "\u2705";
        var incorrectEmoji = "\u274c";
        
        var test = _dataContext.GetTestByChatId(chatId);
        if (test?.CurrentQuestionIndex + 1 < test?.QuestionResults?.Count)
        {
            return false;
        }
        
        var questions = test?.QuestionResults;
        if (questions is null)
        {
            return false;
        }

        var correctAnswersCount = test?.QuestionResults?.Where(q => q.IsCorrectAnswer(q.UserAnswer ?? "")).Count();
        foreach (var question in questions)
        {
            var emoji = question.IsCorrectAnswer(question.UserAnswer ?? "") ? correctAnswerEmoji : incorrectEmoji;
            
            result.AppendLine($"Вопрос: {question.Question}");
            result.AppendLine($"Правильный ответ: {question.CorrectAnswer} {emoji}");
            result.AppendLine("\n");
        }

        result.AppendLine("\n");
        result.AppendLine($"Вы ответили правильно {correctAnswersCount}/{test?.QuestionResults?.Count}");

        await BotClient.SendTextMessageAsync(
            chatId: chatId,
            text: result.ToString(),
            cancellationToken: cancellationToken);
        
        FinishTesting(chatId);
        return true;
    }

    private Task SetAnswerToQuestion(long chatId, string answerIndex)
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
        
        currentQuestion.UserAnswer = currentQuestion.Options?[int.Parse(answerIndex) - 1].Option;
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

    private void FinishTesting(long chatId)
    {
        _dataContext.RemoveTest(chatId);
    }
}
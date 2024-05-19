using System.Text;
using StudentHelper.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentHelper.Services.Commands;

public class NewLectureCommand : TelegramBotCommand
{
    private readonly IQuizService _quizService;
    
    public NewLectureCommand(
        ITelegramBotClient botClient,
        IQuizService quizService) : base(botClient)
    {
        _quizService = quizService;
    }

    public override async Task ExecuteCommand(Message message, CancellationToken cancellationToken)
    {
        var newLecture = await _quizService.GenerateNewQuiz(message.Text);
        await BotClient.SendTextMessageAsync(message.Chat.Id, newLecture.Lecture);

        var answer = new StringBuilder();
        answer.AppendLine("\n");
        var optionIndex = 1;
        foreach (var question in newLecture.Questions)
        {
            answer.AppendLine(question.Question);
            foreach (var option in question.Options)
            {
                answer.AppendLine($"{optionIndex}) {option.Option}");
            }

            answer.AppendLine($"Правильный ответ: {question.Answer}");
            answer.AppendLine("\n");
            answer.AppendLine("\n");
            answer.AppendLine("\n");
            optionIndex++;
        }
        await BotClient.SendTextMessageAsync(message.Chat.Id, answer.ToString());

    }
}
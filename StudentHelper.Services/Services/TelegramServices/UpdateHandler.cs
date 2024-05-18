using System.Text;
using StudentHelper.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace StudentHelper.Services.Services.TelegramServices;

public class UpdateHandler : IUpdateHandler
{
    private readonly IQuizService _quizService;
    
    public UpdateHandler(IQuizService quizService)
    {
        _quizService = quizService;
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    var message = update.Message;
                    Console.WriteLine($"Received a '{message.Text}' message in chat.");

                    var chat = message.Chat;
                    switch (message.Type)
                    {
                        case MessageType.Text:
                        {
                            if (message.Text == "/start")
                            {
                                await botClient.SendTextMessageAsync(
                                    chat.Id,
                                    "Привет, я бот по помощи студентам studenthelper!" +
                                    "Я помогу тебе с решением любой задачи.");
                                return;
                            }
                            
                            var newLecture = await _quizService.GenerateNewQuiz(message.Text);
                            await botClient.SendTextMessageAsync(chat.Id, newLecture.Lecture);

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
                            }
                            await botClient.SendTextMessageAsync(chat.Id, answer.ToString());

                            return;
                        }
                        default:
                        {
                            await botClient.SendTextMessageAsync(
                                chat.Id,
                                "Хуй!");
                            return;
                        }
                    }
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
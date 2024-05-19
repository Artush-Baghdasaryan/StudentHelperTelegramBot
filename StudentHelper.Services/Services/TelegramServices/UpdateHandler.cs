using System.Text;
using Microsoft.Extensions.Options;
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
    private Dictionary<long, int> _currentQuestionIndex = new();
    private Dictionary<long, Quiz> _currentQuizzes = new();

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
                                            "Привет, я бот по помощи студентам studenthelper!\n" +
                                            "Я помогу тебе с решением любой задачи!\n" +
                                            "Напиши тему для теста!");
                                        return;
                                    }

                                    if (message.Text == "Начать тест")
                                    {
                                        if (_currentQuizzes.ContainsKey(chat.Id))
                                        {
                                            _currentQuestionIndex[chat.Id] = 0;
                                            await SendNextQuestion(botClient, chat.Id);
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chat.Id, "Сначала укажите тему для теста.");
                                        }
                                        return;
                                    }

                                    if (_currentQuizzes.ContainsKey(chat.Id))
                                    {
                                        var quiz = _currentQuizzes[chat.Id];
                                        var questionIndex = _currentQuestionIndex[chat.Id];

                                        if (questionIndex < quiz.Questions.Count)
                                        {
                                            var question = quiz.Questions[questionIndex];
                                            var isCorrect = message.Text == question.Answer;
                                            await botClient.SendTextMessageAsync(chat.Id, isCorrect ? "Правильно!" : "Неправильно!");

                                            _currentQuestionIndex[chat.Id]++;
                                            await SendNextQuestion(botClient, chat.Id);
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chat.Id, "Тест завершен!");
                                        }
                                    }
                                    else
                                    {
                                        var newQuiz = await _quizService.GenerateNewQuiz(message.Text);
                                        _currentQuizzes[chat.Id] = newQuiz;
                                        _currentQuestionIndex[chat.Id] = 0;

                                        var replyKeyboard = new ReplyKeyboardMarkup(new[]
                                            {
                                                new KeyboardButton[] { "Начать тест" }
                                            })
                                        {
                                            ResizeKeyboard = true
                                        };

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            newQuiz.Lecture,
                                            replyMarkup: replyKeyboard
                                        );
                                    }
                                    return;
                                }
                            default:
                                {
                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Я понимаю только текстовые сообщения.");
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

    private async Task SendNextQuestion(ITelegramBotClient botClient, long chatId)
    {
        if (_currentQuizzes.TryGetValue(chatId, out var quiz))
        {
            var questionIndex = _currentQuestionIndex[chatId];
            if (questionIndex < quiz.Questions.Count)
            {
                var question = quiz.Questions[questionIndex];
                var options = question.Options.Select((o, i) => new KeyboardButton($"{i + 1}) {o.Option}")).ToArray();
                var replyKeyboard = new ReplyKeyboardMarkup(options)
                {
                    ResizeKeyboard = true
                };

                await botClient.SendTextMessageAsync(
                    chatId,
                    question.Question,
                    replyMarkup: replyKeyboard
                );
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId, "Тест завершен!");
                _currentQuizzes.Remove(chatId);
                _currentQuestionIndex.Remove(chatId);
            }
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

using System.Net.NetworkInformation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StudentHelper.Services.Interfaces;
using StudentHelper.Services.Models;

namespace StudentHelper.Services.Services.TelegramServices;

public class QuizService : IQuizService
{
    private readonly IAiPromptService _aiPromptService;
    private readonly IChatGptService _chatGptService;
    
    public QuizService(
        IAiPromptService aiPromptService,
        IChatGptService chatGptService)
    {
        _aiPromptService = aiPromptService;
    }

    public async Task<Quiz> GenerateNewQuiz(string quizTheme)
    {
        var lecture = await _aiPromptService.GenerateNewLecture(quizTheme);
        var questions = await _aiPromptService.GenerateQuestionsForLecture(lecture);
        var generatedQuestions = JsonConvert.DeserializeObject<GeneratedQuestionsDto>(questions);
        var quiz = new Quiz
        {
            Lecture = lecture,
            Questions = generatedQuestions?.Questions
        };
        return quiz ?? throw new InvalidOperationException();
    }
}
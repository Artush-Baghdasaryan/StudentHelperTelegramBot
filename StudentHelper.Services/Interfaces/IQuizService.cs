using Newtonsoft.Json.Linq;
using StudentHelper.Services.Models;

namespace StudentHelper.Services.Interfaces;

public interface IQuizService
{
    Task<Quiz> GenerateNewQuiz(string quizTheme);
}
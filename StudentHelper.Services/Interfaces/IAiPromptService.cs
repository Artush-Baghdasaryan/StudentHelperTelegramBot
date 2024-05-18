using Newtonsoft.Json.Linq;
using StudentHelper.Services.Models;

namespace StudentHelper.Services.Interfaces;

public interface IAiPromptService
{
    Task<string> GenerateQuestionsForLecture(string lecture);
    Task<string> GenerateNewLecture(string lectureTheme);
}
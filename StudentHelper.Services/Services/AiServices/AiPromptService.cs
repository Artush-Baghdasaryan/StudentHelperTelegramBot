using Newtonsoft.Json.Linq;
using StudentHelper.Services.Interfaces;
using StudentHelper.Services.Models;
using StudentHelper.Services.PromptModels;

namespace StudentHelper.Services.Services.GigachatServices;

public class AiPromptService : IAiPromptService
{
    private readonly IGigachatService _gigachatService;
    private readonly IChatGptService _chatGptService;
    
    public AiPromptService(
        IGigachatService gigachatService,
        IChatGptService chatGptService)
    {
        _gigachatService = gigachatService;
        _chatGptService = chatGptService;
    }
    
    public async Task<string> GenerateQuestionsForLecture(string lecture)
    {
        var userContent = $"{BasePrompts.GenerateQuestionsSystem} {QuestionPrompt.Prompt}";
        var messages = new JArray
        {
            new JObject
            {
                { "role", "assistant" },
                { "content", lecture },
            },
            new JObject
            {
                { "role", "user" },
                { "content", userContent }
            }
        };

        var questions = await _chatGptService.SendRequest(messages);
        return questions ?? throw new InvalidOperationException("Что по пошло не так, попробуйте пожалуйста еще раз!");
    }

    public async Task<string> GenerateNewLecture(string lectureTheme)
    {
        var messages = new JArray
        {
            new JObject
            {
                { "role", "system" },
                { "content", BasePrompts.GenerateNewLectureSystem }
            },
            new JObject
            {
                {"role", "user"},
                {"content", lectureTheme}
            }
        };

        var newLecture = await _chatGptService.SendRequest(messages);
        return newLecture ?? throw new InvalidOperationException("Что по пошло не так, попробуйте пожалуйста еще раз!");
    }

    private string GetJsonContentForPrompt(JToken messages)
    {
        var promptModel = new JObject
        {
            { "model", "GigaChat:latest" },
            {
                "messages", messages
            },
            { "temperature", 1.0 },
            { "top_p", 1.0 },
            { "n", 1 },
            { "stream", false },
            { "max_tokens", 10000 },
        };

        return promptModel.ToString();
    }
}
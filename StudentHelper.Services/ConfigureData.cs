using Microsoft.Extensions.DependencyInjection;
using StudentHelper.Services.Interfaces;
using StudentHelper.Services.Services.AiServices;
using StudentHelper.Services.Services.GigachatServices;
using StudentHelper.Services.Services.TelegramServices;

namespace StudentHelper.Services;

public static class ConfigureData
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<IChatGptService, ChatGptService>();
        services.AddSingleton<IGigachatService, GigachatService>();
        services.AddSingleton<IAiPromptService, AiPromptService>();
        services.AddScoped<IQuizService, QuizService>();
    }
}
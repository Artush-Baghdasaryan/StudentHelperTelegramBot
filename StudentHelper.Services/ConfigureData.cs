using System.Data;
using Microsoft.Extensions.DependencyInjection;
using StudentHelper.Services.Commands;
using StudentHelper.Services.Data;
using StudentHelper.Services.Interfaces;
using StudentHelper.Services.Services.AiServices;
using StudentHelper.Services.Services.Data;
using StudentHelper.Services.Services.GigachatServices;
using StudentHelper.Services.Services.TelegramServices;
using Telegram.Bot.Polling;

namespace StudentHelper.Services;

public static class ConfigureData
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<BotDataContext>();
        services.AddSingleton<IChatGptService, ChatGptService>();
        services.AddSingleton<IGigachatService, GigachatService>();
        services.AddSingleton<IAiPromptService, AiPromptService>();
        services.AddScoped<CommandBuilder>();
        
        services.AddScoped<IQuizService, QuizService>();
        services.AddScoped<IReceiverService, ReceiverService>();
        services.AddScoped<IUpdateHandler, UpdateHandler>();
    }
}
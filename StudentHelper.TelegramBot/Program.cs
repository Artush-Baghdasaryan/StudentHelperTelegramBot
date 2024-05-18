using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StudentHelper.Services;
using StudentHelper.Services.Interfaces;
using StudentHelper.Services.Models;
using StudentHelper.Services.Services.GigachatServices;
using StudentHelper.Services.Settings;
using Telegram.Bot;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<GigachatApiConfiguration>(context.Configuration
            .GetSection(GigachatApiConfiguration.ConfigurationSectionName));
        services.Configure<TelegramBotConfiguration>(context.Configuration
            .GetSection(TelegramBotConfiguration.ConfigurationSectionName));
        services.Configure<ChatGptConfiguration>(context.Configuration
            .GetSection(nameof(ChatGptConfiguration)));

        services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    var botConfig = sp.GetService<IOptions<TelegramBotConfiguration>>()!.Value;
                    TelegramBotClientOptions options = new(botConfig.Token);
                    return new TelegramBotClient(options, httpClient);
                });

        services.AddHttpClient<IGigachatService, GigachatService>(client =>
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
            .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            });
        
        services.ConfigureServices();
    })
    .Build();

var quizService = host.Services.GetRequiredService<IQuizService>();
var botConfig = host.Services.GetRequiredService<IOptions<TelegramBotConfiguration>>()!.Value;

while (true)
{
    Console.Write("Enter theme: ");
    var lectureTheme = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(lectureTheme))
    {
        break;
    }
    
    var result = await quizService.GenerateNewQuiz(lectureTheme);
    ConsoleWriteQuiz(result);
}

static void ConsoleWriteQuiz(Quiz quiz)
{
    Console.WriteLine(quiz.Lecture);
    if (quiz.Questions is null)
    {
        return;
    }
    
    foreach (var question in quiz.Questions)
    {
        Console.WriteLine("Question:   ", question.Question);
        foreach (var option in question.Options)
        {
            Console.WriteLine(option.Option);
        }
    }    
}

await host.RunAsync();
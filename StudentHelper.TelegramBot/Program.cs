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
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;



class Program
{

    private static ITelegramBotClient _botClient;

    private static ReceiverOptions _receiverOptions;

    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        var message = update.Message;

                        var user = message.From;

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
                                    else
                                    {
                                        await botClient.SendTextMessageAsync(chat.Id, message.Text);
                                    }

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

                        return;
                    }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    static async Task Main(string [] args)
    {
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

        _botClient = new TelegramBotClient("7104954159:AAEo_7WZVfPonHtUsgd4w3I2vIeuLOoLqhI");
        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[]
            {
                UpdateType.Message,
                UpdateType.CallbackQuery
            },
            ThrowPendingUpdates = true,
        };

        using var cts = new CancellationTokenSource();

        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);

        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"Start listening for @{me.Username}");

        await host.RunAsync();
    }
}


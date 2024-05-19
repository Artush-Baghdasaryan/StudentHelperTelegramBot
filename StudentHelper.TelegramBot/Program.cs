using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StudentHelper.Services;
using StudentHelper.Services.Interfaces;
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


try
{
    using var cts = new CancellationTokenSource();
    var receiver = host.Services.GetRequiredService<IReceiverService>();
    await receiver.ReceiveAsync(cts.Token);
}
catch (Exception e)
{
    Console.WriteLine($"Exception caught {e.Message} {e.StackTrace}");
}

await host.RunAsync();


namespace StudentHelper.Services.Settings;

public class TelegramBotConfiguration
{
    public static readonly string ConfigurationSectionName = "TelegramBotConfiguration";

    public string Token { get; set; } = "";
}
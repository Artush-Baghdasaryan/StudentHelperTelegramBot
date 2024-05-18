namespace StudentHelper.Services.Settings;

public class ChatGptConfiguration
{
    public static readonly string ConfigurationSectionName = "ChatGptConfiguration";
    
    public string? Url { get; set; }
    public string? ApiKey { get; set; }
    public string? Model { get; set; }
}
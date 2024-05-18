namespace StudentHelper.Services.Settings;

public class GigachatApiConfiguration
{
    public static readonly string ConfigurationSectionName = "GigachatApiConfiguration";
    
    public string AuthData { get; set; } = "";
    public string RqUID { get; set; } = "";
    public string ApiUrlV1 { get; set; } = "";
    public string ApiUrlV2 { get; set; } = "";
}
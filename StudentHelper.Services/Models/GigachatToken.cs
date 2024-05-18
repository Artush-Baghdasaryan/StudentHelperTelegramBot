using System.Text.Json.Serialization;

namespace StudentHelper.Services.Models;

public class GigachatToken
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("expires_at")]
    public long ExpiresAt { get; set; }

    public bool IsExpired
    {
        get
        {
            var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(ExpiresAt);
            return dateTime < DateTimeOffset.Now;
        }
    }
}
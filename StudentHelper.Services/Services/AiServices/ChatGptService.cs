using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using StudentHelper.Services.Interfaces;
using StudentHelper.Services.Settings;

namespace StudentHelper.Services.Services.AiServices;

public class ChatGptService : IChatGptService
{
    private readonly HttpClient _httpCLient = new();
    private readonly ChatGptConfiguration _configuration;

    public ChatGptService(IOptions<ChatGptConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    public async Task<string> SendRequest(JToken messages)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, CreateUrl("chat/completions"));
        SetHeaders(request);

        request.Content = new StringContent(GetContentData(messages), Encoding.UTF8, "application/json");
        using var response = await _httpCLient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = JObject.Parse(await response.Content.ReadAsStringAsync());
        var contentString = responseContent.SelectToken("choices")?[0]?.SelectToken("message.content");

        return contentString?.Value<string>() ?? throw new Exception("something went wrong");
    }

    private void SetHeaders(HttpRequestMessage request)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _configuration.ApiKey);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
    
    protected string GetContentData(JToken messages)
    {
        var jObject = new JObject
        {
            { "model", _configuration.Model },
            { "messages", messages }
        };
        
        var bodyContent = jObject.ToString();
        return bodyContent;
    }

    private string CreateUrl(string url)
    {
        return $"{_configuration.Url}/{url}";
    }

}
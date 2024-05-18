using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using StudentHelper.Services.Interfaces;
using StudentHelper.Services.Models;
using StudentHelper.Services.Settings;

namespace StudentHelper.Services.Services.GigachatServices;

public class GigachatService : IGigachatService
{
    private readonly GigachatApiConfiguration _gigachatSettings;
    private readonly HttpClient _httpClient;
    private GigachatToken _gigachatToken = null!;
        
    public GigachatService(
        IOptions<GigachatApiConfiguration> gigachatSettings
    )
    {
        _gigachatSettings = gigachatSettings.Value;

        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        _httpClient = new HttpClient(clientHandler);
    }

    public async Task<string?> SendMessage(string promptContent)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_gigachatSettings.ApiUrlV1}/chat/completions");
        await SetHeaders(request);

        request.Content = new StringContent(promptContent, Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
        var content = responseObject.SelectToken("choices")?[0]?.SelectToken("message.content");
        return content?.Value<string>();
    }

    public async Task<byte[]> GetImageBytes(string fileId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_gigachatSettings.ApiUrlV1}/files/{fileId}/content");
        await SetHeaders(request);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jpg"));
        using var response = await _httpClient.SendAsync(request);
        return await response.Content.ReadAsByteArrayAsync();
    }
        
    private async Task<GigachatToken> RequestToken()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_gigachatSettings.ApiUrlV2}/oauth");
        request.Headers.Add("RqUID", _gigachatSettings.RqUID);
        var formData = new List<KeyValuePair<string, string>>
        {
            new("scope", "GIGACHAT_API_PERS")
        };

        request.Content = new FormUrlEncodedContent(formData);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _gigachatSettings.AuthData);

        using var response = await _httpClient.SendAsync(request);
        try
        {
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStreamAsync();
            var gigachatToken = await JsonSerializer.DeserializeAsync<GigachatToken>(responseContent);

            return gigachatToken!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something went wrong while receiving token from GIGACHAT : rquest = {request}, response = {response}  exception {ex}");
            return null!;
        }
    }

    private async Task SetHeaders(HttpRequestMessage request)
    {
        var token = await GetGigachatToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private async Task<string?> GetGigachatToken()
    {
        var token = _gigachatToken?.AccessToken;
        if (_gigachatToken is not null && !_gigachatToken.IsExpired)
        {
            return _gigachatToken.AccessToken!;
        }
            
        var newToken = await RequestToken();
        _gigachatToken = newToken;
        return _gigachatToken?.AccessToken;
    }
}
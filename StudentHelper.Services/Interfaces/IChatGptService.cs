using Newtonsoft.Json.Linq;

namespace StudentHelper.Services.Interfaces;

public interface IChatGptService
{
    Task<string> SendRequest(JToken messages);
}
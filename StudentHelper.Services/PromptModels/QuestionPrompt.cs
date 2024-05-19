using Newtonsoft.Json.Linq;

namespace StudentHelper.Services.PromptModels;

public class QuestionPrompt
{
    private static readonly JObject PromptObject = new()
    {
        { "question", "" },
        {
            "options", new JArray
            {
                new JObject
                {
                    { "option", "" }
                },
                new JObject
                {
                    { "option", "" }
                },
                new JObject
                {
                    { "option", "" }
                }
            }
        },
        {"correctAnswer", ""}
    };

    public static string Prompt => PromptObject.ToString();
}
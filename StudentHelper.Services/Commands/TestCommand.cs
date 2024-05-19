using System.Text;
using StudentHelper.Services.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StudentHelper.Services.Commands;

public class TestCommand : TelegramBotCommand
{
    private BotDataContext _dataContext;
    
    public TestCommand(
        ITelegramBotClient botClient,
        BotDataContext dataContext) : base(botClient)
    {
        _dataContext = dataContext;
    }

    public override async Task ExecuteCommand(Message message, CancellationToken cancellationToken)
    {
        StartTestIfNeed(message.Chat.Id);

        var test = _dataContext.GetTestByChatId(message.Chat.Id);
        var currentQuestion = test?.QuestionResults?[test.CurrentQuestionIndex];
        if (currentQuestion?.Options is null)
        {
            return;
        }

        var question = new StringBuilder();
        var optionButtons = new List<KeyboardButton>();
        
        var optionIndex = 1;
        question.AppendLine(currentQuestion.Question);
        foreach (var option in currentQuestion.Options)
        {
            optionButtons.Add(new KeyboardButton(optionIndex.ToString()));
            question.AppendLine($"{optionIndex++}) {option.Option}");
        }

        var replyKeyboard = new ReplyKeyboardMarkup(optionButtons)
        {
            ResizeKeyboard = true
        };

        await BotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: question.ToString(),
            replyMarkup: replyKeyboard,
            cancellationToken: cancellationToken);
        
    }

    private void StartTestIfNeed(long chatId)
    {
        var test = _dataContext.GetTestByChatId(chatId);
        if (test is null || test.TestStarted)
        {
            return;
        }

        test.TestStarted = true;
        _dataContext.UpdateTest(chatId, test);
    }

}
using AutoMapper;
using StudentHelper.Services.Models;

namespace StudentHelper.Services.Data;

public class BotDataContext
{
    private readonly IMapper _mapper;

    private readonly Dictionary<long, TestResult> _tests = new();
    
    public BotDataContext()
    {
        _mapper = new MapperConfiguration(config =>
        {
            config.CreateMap<QuestionDto, QuestionResult>();
            config.CreateMap<QuestionResult, QuestionDto>();
        }).CreateMapper();
    }

    public void AddEntity(long chatId, Quiz quiz)
    {
        _tests.Remove(chatId);
        var testResult = new TestResult
        {
            Lecture = quiz.Lecture,
            QuestionResults = _mapper.Map<List<QuestionResult>>(quiz.Questions),
            ChatState = ChatState.Lecture,
            CurrentQuestionIndex = 0
        };
        _tests.Add(chatId, testResult);
    }

    public void UpdateTest(long chatId, TestResult test)
    {
        if (_tests.ContainsKey(chatId))
        {
            _tests[chatId] = test;
        }
    }
    
    public TestResult? GetTestByChatId(long chatId)
    {
        return _tests.GetValueOrDefault(chatId);
    }

    public void RemoveTest(long chatId)
    {
        _tests.Remove(chatId);
    }
}
namespace StudentHelper.Services.Models;

public class Quiz
{
    public string? Lecture { get; set; }
    public IList<QuestionDto>? Questions { get; set; }
}

public class QuestionDto {
    public string? Question { get; set; }
    public IList<OptionDto>? Options { get; set; }
    public string? CorrectAnswer { get; set; }

    public bool IsCorrectAnswer(string answer) => String.Equals(answer, CorrectAnswer);
}

public class TestResult
{
    public string? Lecture { get; set; }
    public IList<QuestionResult>? QuestionResults { get; set; }
    public int CurrentQuestionIndex { get; set; }
    public ChatState ChatState { get; set; }
}

public enum ChatState
{
    None,
    Lecture,
    Test,
}

public class QuestionResult : QuestionDto
{
    public string? UserAnswer { get; set; }
}

public class OptionDto
{
    public string? Option { get; set; }
}

public class GeneratedQuestionsDto
{
    public IList<QuestionDto>? Questions { get; set; }
}
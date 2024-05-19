namespace StudentHelper.Services.Models;

public class Quiz
{
    public string? Lecture { get; set; }
    public IList<QuestionDto>? Questions { get; set; }
}

public class QuestionDto {
    public string? Question { get; set; }
    public IList<OptionDto>? Options { get; set; }
    public string? Answer { get; set; }
} 

public class OptionDto
{
    public string? Option { get; set; }
}

public class GeneratedQuestionsDto
{
    public IList<QuestionDto>? Questions { get; set; }
}
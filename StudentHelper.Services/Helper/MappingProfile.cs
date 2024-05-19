using AutoMapper;
using StudentHelper.Services.Models;

namespace StudentHelper.Services.Helper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<QuestionDto, QuestionResult>();
        CreateMap<QuestionResult, QuestionDto>();
    }
}
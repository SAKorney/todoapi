using AutoMapper;
using TodoApi.Domain;

namespace TodoApi.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateTodoDto, TodoItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.IsCompleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<TodoItem, TodoResponseDto>();
    }
}
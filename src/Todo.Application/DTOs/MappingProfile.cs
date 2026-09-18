using AutoMapper;
using Todo.Domain;

namespace Todo.Application.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TodoItem, TodoResponseDto>();
    }
}

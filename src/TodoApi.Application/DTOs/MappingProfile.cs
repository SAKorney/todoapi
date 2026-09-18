using AutoMapper;
using TodoApi.Domain;

namespace TodoApi.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TodoItem, TodoResponseDto>();
    }
}

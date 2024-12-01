using Data.DTO;
using Data.DTO.EntitiDTO;

public class SearchResultDTO
{
    public List<PostIntroDTO> Posts { get; set; } = new();
    public List<UserDto> Users { get; set; } = new();
    public List<GroupDTO> Groups { get; set; } = new();
} 
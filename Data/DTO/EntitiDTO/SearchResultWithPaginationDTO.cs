using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO.EntitiDTO
{
    public class SearchResultWithPaginationDTO
    {
        public List<PostIntroDTO>? Posts { get; set; } = new();
        public List<UserDto>? Users { get; set; } = new();
        public List<GroupDTO>? Groups { get; set; } = new();
        public int TotalPosts { get; set; }
        public int TotalUsers { get; set; }
        public int TotalGroups { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string? Keyword { get; set; }
    }
}

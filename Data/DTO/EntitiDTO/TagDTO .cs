using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO.EntitiDTO
{
    public class TagDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<PostIntroDTO> PostDTOs { get; set; }
    }
}

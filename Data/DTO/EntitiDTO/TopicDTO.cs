using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO.EntitiDTO
{
    public class TopicDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<GroupDTO> groupDTOs { get; set; }
    }
}

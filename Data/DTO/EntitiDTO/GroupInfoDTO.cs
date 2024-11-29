using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO.EntitiDTO
{
    public class GroupInfoDTO
    {
        public Guid IdGroup { get; set; }
        public string Name { get; set; }
        public string? ImgGroup { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO.EntitiDTO
{
    public class SaveDTO
    {
        public Guid Id { get; set; }
        public Guid IdUser { get; set; }
        public string? SaveName { get; set; }
        public string? FirstImage { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO.EntitiDTO
{
    public class ReportDTO
    {
        public Guid IdPost { get; set; }
        public PostIntroDTO PostIntro { get; set; }

        public int ReportCount { get; set; }
    }
}

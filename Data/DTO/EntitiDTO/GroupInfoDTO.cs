using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO.EntitiDTO
{
    public class GroupInfoDTO
	{
        public Guid Id { get; set; }

        public string Name { get; set; }
		public string Description { get; set; }
		public string? ImgGroup { get; set; }
		public string? ImgCover { get; set; }
		public DateTime DateTime { get; set; }
		public KindGroup StateGroup { get; set; } // Trạng thái của nhóm 
		public Position? PositionUser {get;set;}

        public int MemberCount { get; set;} 
    }
}

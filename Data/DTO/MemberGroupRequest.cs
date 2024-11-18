using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Enums;

namespace Data.DTO
{
	public class MemberGroupRequest
	{
		public string GroupId { get; set; }
		public string MemberId { get; set; }
		public Position Position { get; set; }
	}
}

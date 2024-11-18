using System;
using System.Collections.Generic;
using System.Linq;
using Data.Enums;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO
{
	public class MemberDTO
	{
		public Guid Id { get; set; }
		public string FullName { get; set; }
		public Position Position { get; set; }
	}
}

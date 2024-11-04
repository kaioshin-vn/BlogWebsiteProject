using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO
{
	public class GroupDTO
	{
		[Key]
		public Guid IdGroup { get; set; }
		[Required]
		[MaxLength(21, ErrorMessage = "Tên cộng đồng không được bỏ trống và không quá 21 ký tự.")]
		public string Name { get; set; }
		[Required]
		[MaxLength(500, ErrorMessage = "Mô tả không được bỏ trống và không quá 500 ký tự.")]
		public string Description { get; set; }
		public string? ImgGroup { get; set; }
		public string? ImgCover { get; set; }
		public KindGroup StateGroup { get; set; } // Trạng thái của nhóm 
		public Guid UserId { get; set; }
	}
}

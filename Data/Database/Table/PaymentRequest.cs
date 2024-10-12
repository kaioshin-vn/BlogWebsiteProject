using BlogWebsite.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
	public class PaymentRequest
	{
		[Key]
		public Guid Id { get; set; }
		public Guid? IdUser { get; set; }
        public int OrderId { get; set; }
        public double Amount { get; set; }
		public string? PaymentMethod { get; set; }
        public DateTime CreatedDate { get; set; } 
		[ForeignKey("IdUser")]
		public ApplicationUser? User { get; set; }
	}
}

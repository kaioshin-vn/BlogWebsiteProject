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
	public class PaymentTransaction
	{
		[Key]
		public Guid Id { get; set; }
		public Guid? IdUser { get; set; }
        public bool Success { get; set; }
        public string? PaymentMethod { get; set; }
        public string? OrderId { get; set; }
        public string? PaymentId { get; set; }
        public string? TransactionId { get; set; }
        public string? Token { get; set; }
        public string? VnPayResponseCode { get; set; }
        [ForeignKey("IdUser")]
		public ApplicationUser? User { get; set; }
	}
}

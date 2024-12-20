using BlogWebsite.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class Invoices
    {
        public Guid Id { get; set; }

        public Guid IdUser { get; set; }
        public Guid IdRegis { get; set; }

        public double Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        [ForeignKey("IdUser")]
        public ApplicationUser? User { get; set; }

        [ForeignKey("IdRegis")]
        public RegistrationAdvertisement? RegistrationAdvertisement { get; set; }
    }
}

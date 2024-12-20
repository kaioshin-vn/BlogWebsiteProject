using BlogWebsite.Data;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class RegistrationAdvertisement
    {
        public Guid Id { get; set; }

        public Guid IdUser { get; set; }

        public Guid IdServiceAdvertis { get; set; }

        public string Name { get; set; } 
        public string ContentAds { get; set; }

        public string Link { get;set; }

        public double Price { get; set; }

        public int DurationDays { get; set; }

        public DateTime TimeStart { get; set; }  

        public DateTime TimeAccept { get; set; }

        public WaitState State { get; set; }

        [ForeignKey("IdUser")]
        public ApplicationUser? User { get; set; }

        [ForeignKey("IdServiceAdvertis")]
        public ServiceAdvertisementPricing? ServiceAdvertisementPricing { get; set; }
    }
}

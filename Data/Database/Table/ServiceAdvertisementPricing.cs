using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class ServiceAdvertisementPricing
    {
        public Guid Id { get; set; }

        public Guid IdService { get; set; }   

        public string Name { get; set; }
        public string Description { get; set; }

        public double Price { get; set; }

        public int DurationDays { get; set; }

        [ForeignKey("IdService")]
        public Service? Service { get; set; }
    }
}

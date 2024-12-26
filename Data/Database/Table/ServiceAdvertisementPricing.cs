using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class ServiceAdvertisementPricing
    {
        public Guid Id { get; set; }

        public Guid? IdService { get; set; }

        [Required(ErrorMessage = "Tên không được để trống.")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá không được để trống.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải là số dương.")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Thời gian không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Thời gian phải lớn hơn 0.")]
        public int DurationDays { get; set; }

        public bool IsDelete { get; set; } = false;

        [ForeignKey("IdService")]
        public Service? Service { get; set; }
    }
}

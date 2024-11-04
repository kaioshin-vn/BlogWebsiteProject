using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO.EntitiDTO
{
    public class PostDTO
    {
        public Guid Id { get; set; }

        public Guid IdUser { get; set; }
        public string? Title { get; set; }
        public string? VideoFile { get; set; }
        public string? ImgFile { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? EditDate { get; set; } // lưu lại lịch sử sửa
        public string? Content { get; set; }
    }
}

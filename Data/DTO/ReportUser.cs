using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO
{
    public class UserReport
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Img { get; set; }
        public string Content { get; set; }
    }
}

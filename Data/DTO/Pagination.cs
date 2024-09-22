using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO
{
    public class Pagination<T>
    {
        public List<T> ListPage { get; set; }
        public int TotalPage { get; set; }
    }
}

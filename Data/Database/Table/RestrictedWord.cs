using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class RestrictedWord
    {
        [Key]
        public Guid Id { get; set; }
        public string Word { get; set; }
    }
}

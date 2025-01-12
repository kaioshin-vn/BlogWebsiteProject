using Data.Database.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO
{
    public class PostHideByRestrictedDTO
    {
        public Guid Id { get; set; }

        public Guid IdPost { get; set; }

        public Guid IdRestricted { get; set; }
 
        public Post? Post { get; set; }
                                       
        public string ListRestrictedWord { get; set; }
    }
}

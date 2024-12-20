using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class PostHideByRestricted
    {
        public Guid Id { get; set; }

        public Guid IdPost { get; set; }

        public Guid IdRestricted { get; set; }

        [ForeignKey("IdPost")]
        public Post? Post { get; set; }

        [ForeignKey("IdRestricted")]
        public RestrictedWord? RestrictedWord { get; set; }
    }
}

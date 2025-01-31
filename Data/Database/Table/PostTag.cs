﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class PostTag
    {
        public Guid IdTag { get; set; }
        public Guid IdPost { get; set; }
        [ForeignKey("IdTag")]
        public Tag? Tag { get; set; }
        [ForeignKey("IdPost")]
        public Post? Post { get; set; }
    }
}

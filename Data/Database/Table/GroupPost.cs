﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class GroupPost
    {
        public Guid IdGroup { get; set; }
        public Guid IdPost { get; set; }

        [ForeignKey("IdPost")]
        public Post? Post { get; set; }
        [ForeignKey("IdGroup")]
        public Group? Group { get; set; }
    }
}
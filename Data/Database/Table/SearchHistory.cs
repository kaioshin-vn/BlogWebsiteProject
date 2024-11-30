﻿using BlogWebsite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class SearchHistory
    {
        public Guid Id { get; set; }
        public Guid IdUser { get; set; }
        public string Keyword { get; set; }
        public DateTime SearchDate { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}

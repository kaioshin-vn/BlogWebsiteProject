﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class Service
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}

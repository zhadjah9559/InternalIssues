﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }          //PK
        public string Name { get; set; }     //FK
    }
}

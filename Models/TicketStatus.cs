using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Models
{
    public class TicketStatus
    {
        public int Id { get; set; }          //PK
        public int Name { get; set; }    //FK
    }
}

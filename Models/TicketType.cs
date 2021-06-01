using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Models
{
    public class TicketType
    {
        public int Id { get; set; }          //PK
        //public int TicketId { get; set; }    //FK

        public string Name { get; set; }
    }
}

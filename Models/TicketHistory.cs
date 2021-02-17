using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }          //PK
        public int TicketId { get; set; }    //FK
        public string UserId { get; set; }


        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTimeOffset Created { get; set; }


        //Navigational properties
        public virtual Ticket Ticket { get; set; }    //Parent
        public virtual AppUser User { get; set; }

    }
}

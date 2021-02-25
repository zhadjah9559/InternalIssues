using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Models
{
    public class TicketComment
    {
        public int Id { get; set; }              //PK
        public int TicketId { get; set; }        //FK
        public string AppUserId { get; set; }    //FK      

        [Required]
        [StringLength(300, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long.", MinimumLength = 5)]
        public string CommentBody { get; set; }

        [Display(Name = "Created Date")]
        public DateTimeOffset Created { get; set; }

        [Display(Name = "Created Date")]
        public DateTimeOffset? Updated { get; set; }

        //Navigational properties
        public virtual Ticket Ticket { get; set; }
        public virtual AppUser AppUser { get; set; }

    }
}

using FluentValidation;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Models
{
    public class Ticket
    {
        //overloaded ctor
        public Ticket()
        {
            Comments = new HashSet<TicketComment>();
            Attachments = new HashSet<TicketAttachment>();
            Notifications = new HashSet<Notification>();
            Histories = new HashSet<TicketHistory>();
        }

        public int Id { get; set; }    //PK
       
        //[Required]
        //[StringLength(50)]
        public string Title{ get; set; }

        //[Required]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTimeOffset Created{ get; set; }
        [DataType(DataType.Date)]
        public DateTimeOffset? Updated { get; set; }


        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        public string OwnerUserId { get; set; }
        public string DeveloperUserId { get; set; }

        //Navigational Properties 
        public Project Project { get; set; }
        public TicketType TicketType { get; set; }
        public TicketPriority TicketPriority{ get; set; }   
        public TicketStatus TicketStatus { get; set; }
        public AppUser OwnerUser { get; set; }
        public AppUser DeveloperUser { get; set; }
        public ICollection<TicketAttachment> Attachments { get; set; }
        public ICollection<TicketComment> Comments { get; set; } 
        public ICollection<TicketHistory> Histories { get; set; } 
        public ICollection<Notification> Notifications { get; set; }
    }

    public class TicketValidator : AbstractValidator<Ticket>
    {
        public TicketValidator()
        {
            RuleFor(ticket => ticket.Title).NotNull();
            RuleFor(ticket => ticket.Title).Length<Ticket>(3,50);

            RuleFor(ticket => ticket.Description).NotNull();

            RuleFor(ticket => ticket.Created).Equals(typeof(DateTimeOffset));

            RuleFor(ticket => ticket.Updated).Equals(typeof(DateTimeOffset));



        }
    }



}

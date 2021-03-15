using InternalIssues.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Models
{
    public class Project
    {
        //overloaded ctor
        public Project()
        {
            Members = new HashSet<AppUser>();
            Tickets = new HashSet<Ticket>();
        }

        public int Id { get; set; }     //PK

        [Required]
        [StringLength(50)]
        [Display(Name = "Project Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Select Image")]
        [NotMapped]
        [DataType(DataType.Upload)]
        [MaxFileSize(2*1024*1024)]
        [AllowedExtensionsAttribute(new string[] { ".jpg", ".png" })]
        public IFormFile ImageFormfile { get; set; }
        public string ImageFileName { get; set; }
        public byte[] ImageFileData { get; set; }
        public int? CompanyId { get; set; }

        //Navigational Properties
        //Nav properties are the gateway to getting information from tables (Tickets table, Notifs table, etc)
        public virtual ICollection<AppUser> Members { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual Company Company { get; set; }
    }
}

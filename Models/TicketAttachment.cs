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
    public class TicketAttachment
    {
        public int Id { get; set; }          //PK
        public int TicketId { get; set; }    //FK
        public string UserId { get; set; }   //FK


        [Display(Name = "Select Name")]
        [NotMapped]
        [DataType(DataType.Upload)]
        [MaxFileSize(2*1024*1024)]
        [AllowedExtensionsAttribute(new string[] { ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf" })]
        public IFormFile FormFile { get; set; }
        public string FileName { get; set; }
        public byte[] FileData {get; set; }

        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }


        public virtual Ticket Ticket { get; set; }
        public virtual AppUser User { get; set; }

    }
}

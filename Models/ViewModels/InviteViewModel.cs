using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Models.ViewModels
{
    public class InviteViewModel
    {        
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(60)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(60)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Project Description")]
        public string ProjectDescription { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Company Description")]
        public string CompanyDescription { get; set; }

    }
}

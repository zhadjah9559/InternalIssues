using FluentValidation;
using InternalIssues.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Models
{
    public class AppUser : IdentityUser
    {
        public int CompanyId { get; set; }      //FK

        //[Required]
        //[StringLength(50)]
        public string FirstName { get; set; }

        //[Required]
        //[StringLength(50)]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        [NotMapped]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        [Display(Name = "Select Image")]
        [NotMapped]
        [DataType(DataType.Upload)]
        [MaxFileSize(2 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        public IFormFile AvatarFormFile { get; set; }
        public string AvatarFileName { get; set; }
        public byte[] AvatarFileData { get; set; }

        //Navigational properties
        public Company Company { get; set; }
        public ICollection<Project> Projects { get; set; }
    }

    public class AppUserValidator : AbstractValidator<AppUser>
    {
        public AppUserValidator()
        {
            RuleFor(AppUser => AppUser.FirstName).NotNull();
            RuleFor(AppUser => AppUser.FirstName).Length<AppUser>(3,50);

            RuleFor(AppUser => AppUser.LastName).NotNull();
            RuleFor(AppUser => AppUser.LastName).Length<AppUser>(3,50);
        }
    }
}

using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Models.ViewModels
{
    public class InviteViewModel
    {        
        //[Required]
        //[EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //[Required]
        //[StringLength(60)]
        public string FirstName { get; set; }

        //[Required]
        //[StringLength(60)]
        public string LastName { get; set; }

        //[Required]
        //[StringLength(50)]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        //[Required]
        //[StringLength(50)]
        [Display(Name = "Project Description")]
        public string ProjectDescription { get; set; }

        //[Required]
        //[StringLength(50)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Company Description")]
        public string CompanyDescription { get; set; }
    }

    public class InviteViewModelValidator : AbstractValidator<InviteViewModel>
    {
        public InviteViewModelValidator()
        {
            RuleFor(InviteViewModel => InviteViewModel.Email).NotNull();
            RuleFor(InviteViewModel => InviteViewModel.Email).EmailAddress<InviteViewModel>();

            RuleFor(InviteViewModel => InviteViewModel.FirstName).NotNull();
            RuleFor(InviteViewModel => InviteViewModel.FirstName).Length<InviteViewModel>(3,60);

            RuleFor(InviteViewModel => InviteViewModel.LastName).NotNull();
            RuleFor(InviteViewModel => InviteViewModel.LastName).Length<InviteViewModel>(3,60);

            RuleFor(InviteViewModel => InviteViewModel.ProjectName).NotNull();
            RuleFor(InviteViewModel => InviteViewModel.ProjectName).Length<InviteViewModel>(3,50);

            RuleFor(InviteViewModel => InviteViewModel.ProjectDescription).NotNull();
            RuleFor(InviteViewModel => InviteViewModel.ProjectDescription).Length<InviteViewModel>(3,50);

            RuleFor(InviteViewModel => InviteViewModel.CompanyName).NotNull();
            RuleFor(InviteViewModel => InviteViewModel.CompanyName).Length<InviteViewModel>(3,50);
        }
    }
}

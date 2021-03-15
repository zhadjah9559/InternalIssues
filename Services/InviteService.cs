using InternalIssues.Data;
using InternalIssues.Data.Enums;
using InternalIssues.Models;
using InternalIssues.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Services
{
    public class InviteService : IInviteService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRoleService _roleService;
        private readonly IProjectService _projectService;

        public InviteService(ApplicationDbContext context, 
                             UserManager<AppUser> userManager,
                             IRoleService roleService,
                             IProjectService projectService )
        {
            _context = context;
            _userManager = userManager;
            _roleService = roleService;
            _projectService = projectService;
        }

        public async Task<string> InviteWizardAsync(InviteViewModel inviteViewModel)
        {
            //Create a new Company - Database
            var companyId = await CreateCompanyAsync(inviteViewModel.CompanyName, inviteViewModel.CompanyDescription);
            //Create a new BTUser - UserManager
            var user = new AppUser
            {
                Email = inviteViewModel.Email,
                UserName = inviteViewModel.Email,
                FirstName = inviteViewModel.FirstName,
                LastName = inviteViewModel.LastName,
                EmailConfirmed = true,
                CompanyId = companyId
            };
            var result = await _userManager.CreateAsync(user, "Xyz$098#");

            //Assign that user to a role - RoleService
            await _roleService.AddUserToRoleAsync(user, Roles.Admin.ToString());
            //Create a new project - Database
            var projectId = await CreateProjectAsync(inviteViewModel.ProjectName, inviteViewModel.ProjectDescription, companyId);
            //Assign that user to that project - ProjectService
            await _projectService.AddUserToProjectAsync(user.Id, projectId);

            return user.Id;
        }

        private async Task<int> CreateCompanyAsync(string companyName, string companyDescription)
        {
            Company company = new Company
            {
                Name = companyName,
                Description = companyDescription
            };
            if (!_context.Companies.Any(c => c.Name == company.Name))
            {
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
            }
            else
            {
                company = _context.Companies.FirstOrDefault(c => c.Name == company.Name);
            }

            return company.Id;
        }

        private async Task<int> CreateProjectAsync(string projectName, string projectDescription, int companyId)
        {
            var project = new Project
            {
                Name = projectName,
                Description = projectDescription,
                CompanyId = companyId
            };
            if (!_context.Projects.Any(c => c.Name == project.Name))
            {
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
            }
            else
            {
                project = _context.Projects.FirstOrDefault(c => c.Name == project.Name);
            }

            return project.Id;
        }

        
    }
}

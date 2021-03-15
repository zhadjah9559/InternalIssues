using InternalIssues.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Services
{
    public interface IInviteService
    {
        public Task<string> InviteWizardAsync(InviteViewModel inviteViewModel);

        //private async Task<int> CreateCompanyAsync(string companyName, string companyDescription);

        //private async Task<int> CreateProjectAsync(string projectName, string projectDescription, int companyId);
    }
}

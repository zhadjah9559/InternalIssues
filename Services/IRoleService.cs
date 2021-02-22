using InternalIssues.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Services
{
    public interface IRoleService
    {
        //Method Declarations
        //Everything in interface must be public
        public Task<bool> AddUserToRoleAsync(AppUser user, string roleName);

        public Task<bool> IsUserInRoleAsync(AppUser user,string roleName);

        public Task<IEnumerable<string>> ListUserRolesAsync(AppUser user);

        public Task<bool> RemoveUserFromRoleAsync(AppUser user, string roleName);

        public Task<IEnumerable<AppUser>> UsersInRoleAsync(string RoleName);

        public Task<IEnumerable<AppUser>> UsersNotInRoleAsync(string roleName);

        public IEnumerable<IdentityRole> NonDemoRoles();



    }
}

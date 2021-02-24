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
        //Add/Assign users on the project, return bool to confirm whether it worked
        Task<bool> AddUserToRoleAsync(AppUser user, string roleName);

        //Return a true/false depending on if the specified user 
        Task<bool> IsUserInRoleAsync(AppUser user,string roleName);

        //Return collection of type string displaying the user roles
        Task<IEnumerable<string>> ListUserRolesAsync(AppUser user);

        //Remove a specified user from a role, return bool if successful
        Task<bool> RemoveUserFromRoleAsync(AppUser user, string roleName);

        //Return a collection of all users inside a specified role
        Task<IEnumerable<AppUser>> UsersInRoleAsync(string RoleName);

        //Return a collection of all users not inside of a specified role
        Task<IEnumerable<AppUser>> UsersNotInRoleAsync(string roleName);

        //Return a collection of all roles that aren't created for demonstration purposes
        public IEnumerable<IdentityRole> NonDemoRoles();
    }
}

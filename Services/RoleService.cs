using InternalIssues.Data;
using InternalIssues.Data.Enums;
using InternalIssues.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

                           //inject the UserManager and _context
        public RoleService(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Check whether the user was successfully added to role
        public async Task<bool> AddUserToRoleAsync(AppUser user, string roleName)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }


        public async Task<bool> IsUserInRoleAsync(AppUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }


        public async Task<IEnumerable<string>> ListUserRolesAsync(AppUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }


        public IEnumerable<IdentityRole> NonDemoRoles()
        {
            var roles =  _context.Roles;
            var output = new List<IdentityRole>();

            foreach(var role in roles)
            {
                if( role.Name != Roles.DemoUser.ToString() )
                {
                    output.Add(role);
                }
            }
            return output;
        }


        public async Task<bool> RemoveUserFromRoleAsync(AppUser user, string roleName)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded;
        }


        public async Task<IEnumerable<AppUser>> UsersInRoleAsync(string roleName)
        {
            return await _userManager.GetUsersInRoleAsync(roleName);
        }


        public async Task<IEnumerable<AppUser>> UsersNotInRoleAsync(string roleName)
        {
            var inRole = await UsersInRoleAsync(roleName);
            var users = await _context.Users.ToListAsync();

            return users.Except(inRole);
        }
    }
}

using InternalIssues.Data;
using InternalIssues.Data.Enums;
using InternalIssues.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Services
{
    public class ProjectService : IProjectService
    {
        //Private variables will allow for more security, protect from Database intrusion, etc
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager; 
        private readonly IHttpContextAccessor _contextAccessor;     
        private readonly IRoleService _roleService;

        //Overload the constructor to implement constructor injection
        public ProjectService(ApplicationDbContext context, 
                              UserManager<AppUser> userManager,
                              IHttpContextAccessor contextAccessor,
                              IRoleService roleService)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _roleService = roleService;
        }

        //Assign users to a project   ENCAPSULATED METHOD
        public async Task AddUserToProjectAsync(string userId, int projectId)
        {
            //First check that they are not already on the project 
            try
            {
                //
                if(!await IsUserOnProjectAsync(userId, projectId))
                {
                    //Get user's record
                    AppUser user = await _userManager.FindByIdAsync(userId);
                    //Project manager is a special case, check for that role first
                    if(await _roleService.IsUserInRoleAsync(user, Roles.ProjectManager.ToString()) )
                    {
                        //find the old project manager and remove them from the project
                        var oldManager = await ProjectManagerOnProjectAsync(projectId);
                        if(oldManager != null)
                        {
                            await RemoveUserFromProjectAsync(oldManager.Id, projectId);
                        }
                    }

                    //Get the project's full record
                    Project project = await _context.Projects.FindAsync(projectId);

                    try
                    {
                        //Add the user record tothe virtual ICollection of the project - 
                        project.Members.Add(user);
                        await _context.SaveChangesAsync();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            catch(Exception ex)
            {
                Debug.WriteLine($"***ERROR*** - Error Adding user to the Projects --> {ex.Message}");
            }
        }

        //Return collection of "Developers" on a specified project
        public async Task<IEnumerable<AppUser>> DevelopersOnProjectAsync(int projectId)
        {
            var developers = await _userManager.GetUsersInRoleAsync(Roles.Developer.ToString());
            var onProject = await UsersOnProjectAsync(projectId);
            var devsOnPorject = developers.Intersect(onProject);

            return devsOnPorject.ToList();

        }

        //Check if a user is on a project
        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            var flag = project.Members.Any(u => u.Id == userId);
            return flag;
        }

        //Return collection of type Project not on a specified project
        public async Task<IEnumerable<Project>> ListUserProjectsAsync(string userId)
        {
            //Check the request to see if the user is an Admin - if so return all projects
            var user = await _userManager.FindByNameAsync(userId);
            if(await _roleService.IsUserInRoleAsync(user, Roles.Admin.ToString()) )
            {
                return await _context.Projects.ToListAsync();
            }

            //Create a container for the records
            var output = new List<Project>();

            //Go through all the projects
            foreach( var project in await _context.Projects.ToListAsync() )
            {
                //If the user is on the project add it to the list
                if( await IsUserOnProjectAsync(userId, project.Id) )
                {
                    output.Add(project);
                }
            }
            
            //send out the list
            return output;
        }

        //Return the project manager on a specified project
        public async Task<AppUser> ProjectManagerOnProjectAsync(int projectId)
        {
            var projectManagers = await _userManager.GetUsersInRoleAsync( Roles.ProjectManager.ToString() );
            var onProject = await UsersOnProjectAsync(projectId);
            var projectManager = projectManagers.Intersect(onProject).FirstOrDefault();

            return projectManager;            
        }

        //Remove users from a project     ENCAPSULATED METHOD
        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            //needs to check for pm - only on allows 
            try
            {
                if (await IsUserOnProjectAsync(userId, projectId))
                {
                    //if user is on project, find the project and user
                    AppUser user = await _userManager.FindByIdAsync(userId);
                    Project project = await _context.Projects.FindAsync(projectId);
                    try
                    {
                        //remove record from many to many table and save changes
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine($"***ERROR*** - Error Adding user to the Projects --> {ex.Message}");
            }
        }

        //Submitters on projects
        public async Task<IEnumerable<AppUser>> SubmitterOnProjectsAsync(int projectId)
        {
            var submitters = await _userManager.GetUsersInRoleAsync( Roles.Submitter.ToString() );
            var onProject = await UsersOnProjectAsync(projectId);
            var devsOnPorject = submitters.Intersect(onProject);

            return devsOnPorject.ToList();
        }

        //Return collection of type user not on a specified project
        public async Task<IEnumerable<AppUser>> UsersNotOnProjectAsync(int projectId)
        {
            var output = new List<AppUser>();
            var queryable = _context.Users;

            //Go through the users
            foreach (var user in await _context.Users.ToListAsync())
            {
                //if a user is not on the project, add them to the list
                if (!await IsUserOnProjectAsync(user.Id, projectId))
                {
                    output.Add(user);
                }
            }

            return output;
        }

        //Return collection of type user on a specified project
        public async Task<IEnumerable<AppUser>> UsersOnProjectAsync(int projectId)
        {
            var output = new List<AppUser>();
            var queryable = _context.Users;
            
            //Go through the users
            foreach ( var user in await _context.Users.ToListAsync() )
            {
                //if a User is on the project, add them to the list
                if( await IsUserOnProjectAsync(user.Id, projectId) )
                {
                    output.Add(user);
                }
            }

            return output;
        }

     
    }
}

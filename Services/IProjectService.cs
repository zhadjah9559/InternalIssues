using InternalIssues.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Services
{
    public interface IProjectService
    {
        //Check if a user is on a project
        public Task<bool> IsUserOnProjectAsync(string userId, int projectId);

        //All users on a project
        public Task<IEnumerable<AppUser>> UsersOnProjectAsync(int projectId);

        //All users not on the specified project
        public Task<IEnumerable<AppUser>> UsersNotOnProjectAsync(int projectId);

        //Assign users to a project   ENCAPSULATED METHOD
        public Task AddUserToProjectAsync(string userId, int projectId);

        //Remove users from a project     ENCAPSULATED METHOD
        public Task RemoveUserFromProjectAsync(string userId, int projectId);

        //All projects for one user
        public Task<IEnumerable<Project>> ListUserProjectsAsync(string userId);

        //Developers on Project
        public Task<IEnumerable<AppUser>> DevelopersOnProjectAsync(int projectId);

        //Submitters on projects
        public Task<IEnumerable<AppUser>> SubmitterOnProjectsAsync(int projectId);

        //Project Managers on Project
        public Task<AppUser> ProjectManagerOnProjectAsync(int projectId);

    }
}

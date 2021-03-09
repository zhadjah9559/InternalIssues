using InternalIssues.Data;
using InternalIssues.Models;
using InternalIssues.Models.ViewModels;
using InternalIssues.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRoleService _roleService;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, 
                              IRoleService roleService,
                              ApplicationDbContext context )
        {
            _logger = logger;
            _roleService = roleService;
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            DashboardViewModel model = new DashboardViewModel();

            var tickets = _context.Tickets.Include(t => t.TicketPriority)
                                        .Include(t => t.TicketStatus)
                                        .Include(t => t.TicketType)
                                        .ToList();

            var projects = _context.Projects.Include(p => p.Company)
                                            .Include(p => p.Members)
                                            .Include(p => p.Members)
                                            .ToList();

            //Set all the info from the tickets and projects variables into the model. Send them to the view
            model.Projects = projects;
            model.Tickets = tickets;

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //HTTPGET
        public IActionResult ManageRoles()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(List<string> userIds, string roleName)
        {
            //Go through all the userIds one at a time
            foreach (var userId in userIds)
            {
                //Use the userId to find the whole user record 
                AppUser user = await _context.Users.FindAsync(userId);
                
                //Make sure the user is not already in the role chosen
                if(! await _roleService.IsUserInRoleAsync(user, roleName))
                {
                    //Find all the roles the user currently occupies
                    var userRoles = await _roleService.ListUserRolesAsync(user);
                    //Go through them one at a time 
                    foreach(var role in userRoles)
                    {
                        //Remove the user from any and all roles they occupy
                        await _roleService.RemoveUserFromRoleAsync(user, role);
                    }

                    //add the user to the chosen roleo
                    await _roleService.AddUserToRoleAsync(user, roleName);

                }
            }

            return View();
        }

        public IActionResult LandingPage()
        {
            return View();
        }

        public IActionResult ManageProfile()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

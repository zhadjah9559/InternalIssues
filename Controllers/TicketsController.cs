using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InternalIssues.Data;
using InternalIssues.Models;
using Microsoft.AspNetCore.Identity;
using InternalIssues.Services;
using Microsoft.AspNetCore.Authorization;

namespace InternalIssues.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHistoryService _historyService;
        private readonly IProjectService _projectService;

        public TicketsController(ApplicationDbContext context,
                                 UserManager<AppUser> userManager,
                                 IHistoryService historyService,
                                 IProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _historyService = historyService;
            _projectService = projectService;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> MyTickets()
        {
            var model = new List<Ticket>();
            //test if admin
            if ( User.IsInRole("Admin") )
            {
                model = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus).ToListAsync();
            }
            else if( User.IsInRole("ProjectManager") ) // pm
            {
                //get the user id
                var userId = _userManager.GetUserId(User);

                //connect to projects table and tickets table based on user id
                //
                var projects = await _projectService.ListUserProjectsAsync(userId);

                //look at each project and each ticket
                model = projects.SelectMany(p => p.Tickets).ToList();
            }
            else if(User.IsInRole("Developer"))  //dev
            {
                //check to see if any of the tickets inside the DB has the same UserId 
                var userId = _userManager.GetUserId(User);
                model = _context.Tickets.Where(t => t.DeveloperUserId == userId).ToList();
            }
            else 
            {
                var userId = _userManager.GetUserId(User);
                model = _context.Tickets.Where(t => t.OwnerUserId == userId).ToList();
            }  //owner
            return View(model);
        }

        public async Task<IActionResult> AssignTickets(int projectId)
        {
            ViewData["TicketsIds"] = new SelectList(_context.Tickets.Where(t => t.ProjectId == projectId), "Id", "Title");
            ViewData["DeveloperIds"] = new SelectList(await _projectService.DevelopersOnProjectAsync(projectId), "Id", "FullName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTickets(int ticketId, string developerId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            ticket.DeveloperUserId = developerId;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = ticketId });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.Attachments)
                .Include(t => t.Comments)
                .Include(t => t.Histories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        public IActionResult Create()
        {
            //Dropdown Lists
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Name");
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id");
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id");
            return View();
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.DeveloperUserId);
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.OwnerUserId);
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticket.TicketStatusId);
            return View(ticket);
        }

        //Overloaded Create action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TicketId,CommentBody")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                ticketComment.Created = DateTime.Now;

                //This gets access to whoever is currently logged in
                ticketComment.AppUserId = _userManager.GetUserId(User);

                _context.Add(ticketComment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Tickets");
            }
            return View(ticketComment);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.DeveloperUserId);
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.OwnerUserId);
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticket.TicketStatusId);
            return View(ticket);
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            //Get Old Ticket
            Ticket oldTicket = await _context.Tickets
                               .Include(t => t.TicketType)
                               .Include(t => t.TicketPriority)
                               .Include(t => t.TicketStatus)
                               .Include(t => t.Project)
                               .Include(t => t.DeveloperUser)
                               .Include(t => t.Histories)
                               .Include(t => t.OwnerUser)
                               .AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();

                    //Add History
                    //Get User Id
                    string userId = _userManager.GetUserId(User);

                    //Get New Ticket
                    Ticket newTicket = await _context.Tickets
                                       .Include(t => t.TicketType)
                                       .Include(t => t.TicketPriority)
                                       .Include(t => t.TicketStatus)
                                       .Include(t => t.Project)
                                       .Include(t => t.DeveloperUser)
                                       .AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

                    //Call History Service
                    await _historyService.AddHistoryAsync(oldTicket, newTicket, userId);

                }


                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.DeveloperUserId);
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.OwnerUserId);
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticket.TicketStatusId);
            return View(ticket);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}

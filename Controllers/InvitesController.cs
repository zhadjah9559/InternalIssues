using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InternalIssues.Data;
using InternalIssues.Models;
using InternalIssues.Models.ViewModels;
using InternalIssues.Services;
using Microsoft.AspNetCore.Identity;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace InternalIssues.Controllers
{
    public class InvitesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IInviteService _inviteService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;

        public InvitesController(ApplicationDbContext context,
                                 IInviteService inviteService,
                                 UserManager<AppUser> userManager,
                                 IEmailSender emailSender)
        {
            _context = context;
            _inviteService = inviteService;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Invite.Include(i => i.Company).Include(i => i.Invitee).Include(i => i.Invitor);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invite = await _context.Invite
                .Include(i => i.Company)
                .Include(i => i.Invitee)
                .Include(i => i.Invitor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invite == null)
            {
                return NotFound();
            }

            return View(invite);
        }

        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Set<Company>(), "Id", "Id");
            ViewData["InviteeId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["InvitorId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }
               
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email, FirstName, LastName, CompanyName, CompanyDescription, ProjectName, ProjectDescription")] InviteViewModel inviteViewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = await _inviteService.InviteWizardAsync(inviteViewModel);
                var companyId = _context.Companies.FirstOrDefault(c => c.Name == inviteViewModel.CompanyName).Id;
                var invite = new Invite
                {
                    Email = inviteViewModel.Email,
                    CompanyId = companyId,
                    InviteDate = DateTime.Now,
                    IsValid = true,
                    InvitorId = _userManager.GetUserId(User),
                    InviteeId = userId,
                    CompanyToken = Guid.NewGuid()
                };
                _context.Add(invite);
                await _context.SaveChangesAsync();

                var code = invite.CompanyToken;
                var callbackUrl = Url.Action(
                    "AcceptInvite",
                    "Tickets",
                    values: new { userId, code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(invite.Email, "Join my Bug Tracker",
                $"Create a ticket in my bug tracker by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToAction(nameof(Index));
            }            
            return View(inviteViewModel);
        }
                
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invite = await _context.Invite.FindAsync(id);
            if (invite == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Set<Company>(), "Id", "Id", invite.CompanyId);
            ViewData["InviteeId"] = new SelectList(_context.Users, "Id", "Id", invite.InviteeId);
            ViewData["InvitorId"] = new SelectList(_context.Users, "Id", "Id", invite.InvitorId);
            return View(invite);
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,Email,CompanyToken,InviteDate,InvitorId,InviteeId,IsValid")] Invite invite)
        {
            if (id != invite.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InviteExists(invite.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Set<Company>(), "Id", "Id", invite.CompanyId);
            ViewData["InviteeId"] = new SelectList(_context.Users, "Id", "Id", invite.InviteeId);
            ViewData["InvitorId"] = new SelectList(_context.Users, "Id", "Id", invite.InvitorId);
            return View(invite);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invite = await _context.Invite
                .Include(i => i.Company)
                .Include(i => i.Invitee)
                .Include(i => i.Invitor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invite == null)
            {
                return NotFound();
            }

            return View(invite);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invite = await _context.Invite.FindAsync(id);
            _context.Invite.Remove(invite);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InviteExists(int id)
        {
            return _context.Invite.Any(e => e.Id == id);
        }
    }
}

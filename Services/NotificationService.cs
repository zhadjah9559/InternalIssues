using InternalIssues.Data;
using InternalIssues.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InternalIssues.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        //_context to get notifications
        //usermnagaer to convert claimsprincipal to appuser

        public NotificationService(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(ClaimsPrincipal currentUser)
        {
            //Conver tthew clIMSPRINCIPAL INTO A aPPuSER - DATABASE DOESNT KNOW CLAIMSPRINCICPAL

            AppUser user = await _userManager.GetUserAsync(currentUser);

            var userNotifications = _context.Notification.Where(n => n.RecipientId == user.Id).Include(n=>n.Sender);
            var unreadNotifications = await userNotifications.Where(n => !n.Viewed).ToListAsync();

            return unreadNotifications;
        
        }
    }
}

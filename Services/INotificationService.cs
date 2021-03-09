using InternalIssues.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InternalIssues.Services
{
    public interface INotificationService
    {
        public Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(ClaimsPrincipal currentUser);

    }
}

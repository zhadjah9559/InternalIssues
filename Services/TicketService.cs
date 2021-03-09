using InternalIssues.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Services
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;

        public TicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetNumberOfAllTickets()
        {
            //Get number of all tickets across all projects
            return await _context.Tickets.CountAsync();
        }

        public async Task<int> GetNumberOfAllAssignedTickets()
        {
            return await _context.Tickets.Where(t => !string.IsNullOrWhiteSpace(t.DeveloperUserId)  ).CountAsync();
        }


        public async Task<int> GetNumberOfAllUnAssignedTickets()
        {
            return await _context.Tickets.Where(t => string.IsNullOrWhiteSpace(t.DeveloperUserId)).CountAsync();
        }

        //Get all the tickets whose TicketStatus = Open 
        public async Task<int> GetNumberOfAllOpenTickets()
        {
            return await _context.Tickets.Where(ts => ts.TicketStatus.Name == "Open").CountAsync();
        }

        public async Task<int> GetNumberOfAllClosedTickets()
        {
            return await _context.Tickets.Where(tt => tt.TicketType.Name == "Closed").CountAsync();
        }
    }
}

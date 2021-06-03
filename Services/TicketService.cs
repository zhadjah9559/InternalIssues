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
            return await _context.Tickets.CountAsync();
        }

        public async Task<int> GetNumberOfAllAssignedTickets()
        {
            return await _context.Tickets.Where(t => t.TicketStatus.Name == "Development" ).CountAsync();
        }

        public async Task<int> GetNumberOfAllUnAssignedTickets()
        {
            var assignedTickets = await GetNumberOfAllAssignedTickets();
            var totalTicketsCount = await _context.Tickets.Where(t => t.Id != 0).CountAsync();

            var unAssignedTickets = totalTicketsCount - assignedTickets;

            return unAssignedTickets;
        }

        //Get all the tickets whose TicketStatus = Open 
        public async Task<int> GetNumberOfAllOpenTickets()
        {
            return await _context.Tickets.Where(t => t.TicketStatus.Name == "Open").CountAsync();
        }

        public async Task<int> GetNumberOfAllClosedTickets()
        {
            return await _context.Tickets.Where(t => t.TicketType.Name == "Closed").CountAsync();
        }
    }
}

using InternalIssues.Data;
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

        public Task<int> GetNumberOfAllTickets()
        {
            //Write linq statement that will get the number of all tickets across all 3 projects
            int numOfTickets;

            return numOfTickets;
        }
    }
}

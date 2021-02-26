using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Services
{
    public interface ITicketService
    {
        //Return the number of all tickets across all projects
        public Task<int> GetNumberOfAllTickets();
    }
}

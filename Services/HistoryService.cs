using InternalIssues.Data;
using InternalIssues.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        //overload ctor and inject DB and email sender. 
        public HistoryService(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }


        
        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            //This method will be called for every possible change that can be made to a ticket. 
            //After validating that the old ticket is different than the new ticket, create a new
            //instance of TicketHistory and add it as a record into the TicketHistory table

            if (oldTicket.Title != newTicket.Title)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "Title",
                    OldValue = oldTicket.ToString(),
                    NewValue = newTicket.Title,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }

            if (oldTicket.Description != newTicket.Description)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "Description",
                    OldValue = oldTicket.Description,
                    NewValue = newTicket.Description,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }

            if (oldTicket.TicketType != newTicket.TicketType)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "TicketType",
                    OldValue = oldTicket.TicketType.Name,
                    NewValue = newTicket.TicketType.Name,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }

            if (oldTicket.TicketPriority != newTicket.TicketPriority)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "TicketPriority",
                    OldValue = oldTicket.TicketPriority.Name,
                    NewValue = newTicket.TicketPriority.Name,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }

            if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "TicketStatus",
                    OldValue = oldTicket.TicketStatus.Name,
                    NewValue = newTicket.TicketStatus.Name,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }

            if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "DeveloperUserId",
                    OldValue = oldTicket.DeveloperUser.FullName,
                    NewValue = newTicket.DeveloperUser.FullName,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }

            if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
            {
                Notification notification = new Notification
                {
                    TicketId = newTicket.Id,
                    Description = "You have a new Ticket.",
                    Created = DateTimeOffset.Now,
                    SenderId = userId,
                    RecipientId = newTicket.DeveloperUserId
                };
                await _context.Notification.AddAsync(notification);

                //Send Email
                string devEmail = newTicket.DeveloperUser.Email;
                string subject = "New Ticket Assignment";
                string message = "$You have a new ticket for project: { newTicket.Project.Name}";

                await _emailSender.SendEmailAsync(devEmail, subject, message);
            }



            //TicketPriority
            //TicketStatus
            //DeveloperUserId   Enhance later
            //  




        }
    }
}

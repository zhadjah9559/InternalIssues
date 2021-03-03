﻿using InternalIssues.Data;
using InternalIssues.Models.ChartModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Controllers
{
    public class ChartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly List<string> _backgroundColors;

        //Create Ctor to inject DB
        public ChartsController(ApplicationDbContext context)
        {
            _context = context;
            _backgroundColors = new List<string>
            {
                "#FF0000",
                "#19FF5A",
                "#FFCE19",
                "#0D22FF",
                "#FE7B00",
                "#19D0FF",
                "#D20DFF",
                "#A0FF19",
                "#00FF06",
                "#000000"
            };
        }

        public JsonResult PriorityChart()
        {
            var result = new ChartJSModel();
            var priorities = _context.TicketPriorities.ToList();
            int count = 0;

            foreach(var priority in priorities)
            {
                result.Labels.Add(priority.Name);
                result.Data.Add(_context.Tickets.Where(t => t.TicketPriorityId == priority.Id).Count());
                
                if(count < 10)
                {
                    result.BackgroundColors.Add(_backgroundColors[count]);
                }
                else
                {
                    result.BackgroundColors.Add(_backgroundColors[count % 10]);
                }
                count++;
            }
            return Json(result);
        }

        public JsonResult TypeChart()
        {
            var result = new ChartJSModel();
            var types = _context.TicketTypes.ToList();
            int count = 0;

            foreach (var type in types)
            {
                result.Labels.Add(type.Name);
                result.Data.Add(_context.Tickets.Where(t => t.TicketTypeId == type.Id).Count());

                if (count < 10)
                {
                    result.BackgroundColors.Add(_backgroundColors[count]);
                }
                else
                {
                    result.BackgroundColors.Add(_backgroundColors[count % 10]);

                }
                count++;
            }

            return Json(result);
        }

        public JsonResult StatusChart()
        {
            var result = new ChartJSModel();
            var statuses = _context.TicketStatuses.ToList();
            int count = 0;

            foreach (var status in statuses)
            {
                result.Labels.Add(status.Name);
                result.Data.Add(_context.Tickets.Where(t => t.TicketStatusId == status.Id).Count());

                if (count < 10)
                {
                    result.BackgroundColors.Add(_backgroundColors[count]);
                }
                else
                {
                    result.BackgroundColors.Add(_backgroundColors[count % 10]);

                }
                count++;
            }
            return Json(result);
        }
    }
}

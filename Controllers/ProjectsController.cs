﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InternalIssues.Data;
using InternalIssues.Models;
using InternalIssues.Services;
using InternalIssues.Data.Enums;

namespace InternalIssues.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProjectService _projectService;
        private readonly IRoleService _roleService;

        public ProjectsController(ApplicationDbContext context,
                                    IProjectService projectService,
                                    IRoleService roleService)
        {
            _context = context;
            _projectService = projectService;
            _roleService = roleService;
        }

        //GET
        public async Task<IActionResult> ManageUsersOnProject()
        {
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Name");
            ViewData["ProjectManagerId"] = new SelectList(await _roleService.UsersInRoleAsync(Roles.ProjectManager.ToString()), "Id", "FullName");
            ViewData["DeveloperIds"] = new MultiSelectList(await _roleService.UsersInRoleAsync(Roles.Developer.ToString()), "Id", "FullName");
            ViewData["SubmitterIds"] = new MultiSelectList(await _roleService.UsersInRoleAsync(Roles.Submitter.ToString()), "Id", "FullName");
            return View();
        }

        //projectId
        //projectManager
        //developerId
        //submitterIds
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUsersOnProject(int projectId, string projectManagerId, 
                                                              List<string> developerIds, List<string> submitterIds)
        {
            var currentlyOnProject = await _projectService.UsersOnProjectAsync(projectId);
            foreach(var user in currentlyOnProject)
            {
                await _projectService.RemoveUserFromProjectAsync(user.Id, projectId);
            }

            await _projectService.AddUserToProjectAsync(projectManagerId, projectId);
            foreach(var userId in developerIds)
            {
                await _projectService.AddUserToProjectAsync(userId, projectId);
            }
            foreach (var userId in developerIds)
            {
                await _projectService.AddUserToProjectAsync(userId, projectId);
            }
            return RedirectToAction();
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Projects.Include(p => p.Company);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ImageFileName,ImageFileData,CompanyId")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Set<Company>(), "Id", "Id", project.CompanyId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Set<Company>(), "Id", "Id", project.CompanyId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageFileName,ImageFileData,CompanyId")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Set<Company>(), "Id", "Id", project.CompanyId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}

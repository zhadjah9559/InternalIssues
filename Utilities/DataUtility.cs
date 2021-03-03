﻿using InternalIssues.Data;
using InternalIssues.Data.Enums;
using InternalIssues.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Utilities
{
    public static class DataUtility
    {
        static int company1Id;
        static int company2Id;
        static int company3Id;

        public static string GetConnectionString(IConfiguration configuration)
        {
            //default connection string will come from appSettings like usual
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            //it will automatically overwritten if we are running on heroku
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            //Provides a simple way to create and maage the contents of connection strings used by the NpgsqlConnection class
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }

        public static async Task ManageDataAsync(IHost Host)
        {
            //This technique is used to obtain references to  services that get registered in the 
            //ConfiguredServices method of the Startup class
            using var svcScope = Host.Services.CreateScope();
            var svcProvider = svcScope.ServiceProvider;

            //This dbContextSvc knows how to talk to the database
            //Service 1: An instance of ApplicationDbContext
            //mechanism to talk to the DB
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();

            //Service 2: An instance of Role Manager
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //Service 3: Instance of UserManager
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<AppUser>>();

            //TsTEP 1: This is the programmatic equivalent to update-Database
            await dbContextSvc.Database.MigrateAsync();

            await SeedRolesAsync(userManagerSvc, roleManagerSvc);
            await SeedDefaultCompaniesAsync(dbContextSvc);
            await SeedDefaultUsersAsync(userManagerSvc, roleManagerSvc);
            await SeedDemoUsersAsync(userManagerSvc, roleManagerSvc);
            await SeedDefaultTicketTypeAsync(dbContextSvc);
            await SeedDefaultTicketStatusAsync(dbContextSvc);
            await SeedDefaultTicketPriorityAsync(dbContextSvc);
            await SeedDefaultProjectsAsync(dbContextSvc);
            await SeedDefaultTicketAsync(dbContextSvc);
        }

        private static async Task SeedRolesAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.ProjectManager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Developer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Submitter.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.NewUser.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.DemoUser.ToString()));
        }

        public static async Task SeedDemoUsersAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Demo Admin User
            var defaultUser = new AppUser
            {
                UserName = "johndoe@coderfoundry.com",
                Email = "johndoe@coderfoundry.com",
                FirstName = "john",
                LastName = "doe",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo Admin User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }

            //Seed Demo ProjectManager User
            defaultUser = new AppUser
            {
                UserName = "demopm@coderfoundry.com",
                Email = "demopm@coderfoundry.com",
                FirstName = "Demo",
                LastName = "ProjectManager",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo ProjectManager1 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Demo Developer User
            defaultUser = new AppUser
            {
                UserName = "demodev@coderfoundry.com",
                Email = "demodev@coderfoundry.com",
                FirstName = "Demo",
                LastName = "Developer",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo Developer1 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Demo Submitter User
            defaultUser = new AppUser
            {
                UserName = "demosub@coderfoundry.com",
                Email = "demosub@coderfoundry.com",
                FirstName = "Demo",
                LastName = "Submitter",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo Submitter User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Demo New User
            defaultUser = new AppUser
            {
                UserName = "demonew@coderfoundry.com",
                Email = "demonew@coderfoundry.com",
                FirstName = "Demo",
                LastName = "NewUser",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo Submitter User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        private static async Task SeedDefaultCompaniesAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Company> defaultcompanies = new List<Company>()
                {
                    new Company() { Name = "Company1", Description="This is Default Company 1"},
                    new Company() { Name = "Company2", Description="This is Default Company 2"},
                    new Company() { Name = "Company3", Description="This is Default Company 3"}
                };

                var dbCompanies = context.Companies.Select(c => c.Name).ToList();
                await context.Companies.AddRangeAsync(defaultcompanies.Where(c => !dbCompanies.Contains(c.Name)));
                context.SaveChanges();

                //Get company Ids
                company1Id = context.Companies.FirstOrDefault(p => p.Name == "Company1").Id;
                company2Id = context.Companies.FirstOrDefault(p => p.Name == "Company2").Id;
                company3Id = context.Companies.FirstOrDefault(p => p.Name == "Company3").Id;
            }

            catch (Exception ex)
            {
                Debug.WriteLine("********* ERROR *********");
                Debug.WriteLine("Error Seeding Company");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*************************");
                throw;
            }
        }

        private static async Task SeedDefaultUsersAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new AppUser
            {
                UserName = "zhadjah@gmail.com",
                Email = "zhadjah@gmail.com",
                FirstName = "Zach",
                LastName = "Hadjah",
                EmailConfirmed = true,
                CompanyId = company1Id
            };

            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Admin User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;

            }

            //Seed Default ProjectManager1 User
            defaultUser = new AppUser
            {
                UserName = "andrew@coderfoundry.com",
                Email = "andrew@coderfoundry.com",
                FirstName = "Andrew",
                LastName = "Russell",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default ProjectManager1 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default ProjectManager2 User
            defaultUser = new AppUser
            {
                UserName = "bobby@coderfoundry.com",
                Email = "bobby@coderfoundry.com",
                FirstName = "Bobby",
                LastName = "Davis",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default ProjectManager2 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer1 User
            defaultUser = new AppUser
            {
                UserName = "jason@coderfoundry.com",
                Email = "jason@coderfoundry.com",
                FirstName = "Jason",
                LastName = "Twichell",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Developer1 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer2 User
            defaultUser = new AppUser
            {
                UserName = "kevin@coderfoundry.com",
                Email = "kevin@coderfoundry.com",
                FirstName = "Kevin",
                LastName = "Doyle",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Developer2 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer3 User
            defaultUser = new AppUser
            {
                UserName = "antonio@coderfoundry.com",
                Email = "antonio@coderfoundry.com",
                FirstName = "Antonio",
                LastName = "Raynor",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Developer3 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer4 User
            defaultUser = new AppUser
            {
                UserName = "nick@coderfoundry.com",
                Email = "nick@coderfoundry.com",
                FirstName = "Nick",
                LastName = "Marascio",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Developer4 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer5 User
            defaultUser = new AppUser
            {
                UserName = "haley@coderfoundry.com",
                Email = "haley@coderfoundry.com",
                FirstName = "Haley",
                LastName = "Dennis",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Developer5 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default Submitter1 User
            defaultUser = new AppUser
            {
                UserName = "tom@coderfoundry.com",
                Email = "tom@coderfoundry.com",
                FirstName = "Tom",
                LastName = "Stark",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Submitter1 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default Submitter2 User
            defaultUser = new AppUser
            {
                UserName = "krystal@coderfoundry.com",
                Email = "krystal@coderfoundry.com",
                FirstName = "Krystal",
                LastName = "Quinn",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Submitter2 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        private static async Task SeedDefaultTicketTypeAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketType> ticketTypes = new List<TicketType>() {
                     new TicketType() { Name = "New Development" },
                     new TicketType() { Name = "Runtime" },
                     new TicketType() { Name = "UI" },
                     new TicketType() { Name = "Maintenance" },
                };

                var dbTicketTypes = context.TicketTypes.Select(c => c.Name).ToList();
                await context.TicketTypes.AddRangeAsync(ticketTypes.Where(c => !dbTicketTypes.Contains(c.Name)));
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Ticket Types.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        private static async Task SeedDefaultTicketStatusAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketStatus> ticketStatuses = new List<TicketStatus>() {
                    new TicketStatus() { Name = "New" },
                    new TicketStatus() { Name = "Open" },
                    new TicketStatus() { Name = "Development" },
                    new TicketStatus() { Name = "Testing" },
                    new TicketStatus() { Name = "Closed" },
                };

                var dbTicketStatuses = context.TicketStatuses.Select(c => c.Name).ToList();
                await context.TicketStatuses.AddRangeAsync(ticketStatuses.Where(c => !dbTicketStatuses.Contains(c.Name)));
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Ticket Statuses.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        private static async Task SeedDefaultTicketPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketPriority> ticketPriorities = new List<TicketPriority>() {
                                                    new TicketPriority() { Name = "Low" },
                                                    new TicketPriority() { Name = "Medium" },
                                                    new TicketPriority() { Name = "High" },
                                                    new TicketPriority() { Name = "Urgent" },
                };

                var dbTicketPriorities = context.TicketPriorities.Select(c => c.Name).ToList();
                await context.TicketPriorities.AddRangeAsync(ticketPriorities.Where(c => !dbTicketPriorities.Contains(c.Name)));
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Ticket Priorities.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        //UNCOMPLETED
        public static async Task SeedDefaultProjectsAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Project> projects = new List<Project>() {
                     new Project() { CompanyId = company1Id, Name = "Build a Personal Porfolio", Description="Single page html, css & javascript page.  Serves as a landing page for candidates and contains a bio and links to all applications and challenges." },
                     new Project() { CompanyId = company2Id, Name = "Build a supplemental Blog Web Application", Description="Candidate's custom built web application using .Net Core with MVC, a postgres database and hosted in a heroku container.  The app is designed for the candidate to create, update and maintain a live blog site."  },
                     new Project() { CompanyId = company3Id, Name = "Build an Issue Tracking Web Application", Description="A custom designed .Net Core application with postgres database.  The application is a multi tennent application designed to track issue tickets' progress.  Implemented with identity and user roles, Tickets are maintained in projects which are maintained by users in the role of projectmanager.  Each project has a team and team members."  },
                };

                var dbProjects = context.Projects.Select(c => c.Name).ToList();
                await context.Projects.AddRangeAsync(projects.Where(c => !dbProjects.Contains(c.Name)));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Projects.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        private static async Task SeedDefaultTicketAsync(ApplicationDbContext context)
        {
            //Get project Ids
            int portfolioId = context.Projects.FirstOrDefault(p => p.Name == "Build a Personal Porfolio").Id;
            int blogId = context.Projects.FirstOrDefault(p => p.Name == "Build a supplemental Blog Web Application").Id;
            int bugtrackerId = context.Projects.FirstOrDefault(p => p.Name == "Build an Issue Tracking Web Application").Id;

            //Get ticket type Ids
            int typeNewDev = context.TicketTypes.FirstOrDefault(p => p.Name == "New Development").Id;
            int typeRuntime = context.TicketTypes.FirstOrDefault(p => p.Name == "Runtime").Id;
            int typeUI = context.TicketTypes.FirstOrDefault(p => p.Name == "UI").Id;
            int typeMaintenance = context.TicketTypes.FirstOrDefault(p => p.Name == "Maintenance").Id;

            //Get ticket priority Ids
            int priorityLow = context.TicketPriorities.FirstOrDefault(p => p.Name == "Low").Id;
            int priorityMedium = context.TicketPriorities.FirstOrDefault(p => p.Name == "Medium").Id;
            int priorityHigh = context.TicketPriorities.FirstOrDefault(p => p.Name == "High").Id;
            int priorityUrgent = context.TicketPriorities.FirstOrDefault(p => p.Name == "Urgent").Id;

            //Get ticket status Ids
            int statusNew = context.TicketStatuses.FirstOrDefault(p => p.Name == "New").Id;
            int statusOpen = context.TicketStatuses.FirstOrDefault(p => p.Name == "Open").Id;
            int statusDev = context.TicketStatuses.FirstOrDefault(p => p.Name == "Development").Id;
            int statusTest = context.TicketStatuses.FirstOrDefault(p => p.Name == "Testing").Id;
            int statusClosed = context.TicketStatuses.FirstOrDefault(p => p.Name == "Closed").Id;


            try
            {
                //List of hardcoded tickets
                IList<Ticket> tickets = new List<Ticket>() {
                                //PORTFOLIO
                                new Ticket() {Title = "Portfolio Ticket 1", Description = "Ticket details for portfolio ticket 1", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Portfolio Ticket 2", Description = "Ticket details for portfolio ticket 2", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityMedium, TicketStatusId = statusOpen, TicketTypeId = typeMaintenance},
                                new Ticket() {Title = "Portfolio Ticket 3", Description = "Ticket details for portfolio ticket 3", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev, TicketTypeId = typeUI},
                                new Ticket() {Title = "Portfolio Ticket 4", Description = "Ticket details for portfolio ticket 4", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityUrgent, TicketStatusId = statusTest, TicketTypeId = typeRuntime},
                                new Ticket() {Title = "Portfolio Ticket 5", Description = "Ticket details for portfolio ticket 5", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Portfolio Ticket 6", Description = "Ticket details for portfolio ticket 6", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityMedium, TicketStatusId = statusOpen, TicketTypeId = typeMaintenance},
                                new Ticket() {Title = "Portfolio Ticket 7", Description = "Ticket details for portfolio ticket 7", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev, TicketTypeId = typeUI},
                                new Ticket() {Title = "Portfolio Ticket 8", Description = "Ticket details for portfolio ticket 8", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityUrgent, TicketStatusId = statusTest, TicketTypeId = typeRuntime},
                                //BLOG
                                new Ticket() {Title = "Blog Ticket 1", Description = "Ticket details for blog ticket 1", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusOpen, TicketTypeId = typeRuntime},
                                new Ticket() {Title = "Blog Ticket 2", Description = "Ticket details for blog ticket 2", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusDev, TicketTypeId = typeUI},
                                new Ticket() {Title = "Blog Ticket 3", Description = "Ticket details for blog ticket 3", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeMaintenance},
                                new Ticket() {Title = "Blog Ticket 4", Description = "Ticket details for blog ticket 4", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusOpen, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Blog Ticket 5", Description = "Ticket details for blog ticket 5", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusDev,  TicketTypeId = typeRuntime},
                                new Ticket() {Title = "Blog Ticket 6", Description = "Ticket details for blog ticket 6", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusNew,  TicketTypeId = typeUI},
                                new Ticket() {Title = "Blog Ticket 7", Description = "Ticket details for blog ticket 7", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusOpen, TicketTypeId = typeMaintenance},
                                new Ticket() {Title = "Blog Ticket 8", Description = "Ticket details for blog ticket 8", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusDev,  TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Blog Ticket 9", Description = "Ticket details for blog ticket 9", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusNew,  TicketTypeId = typeRuntime},
                                new Ticket() {Title = "Blog Ticket 10", Description = "Ticket details for blog ticket 10", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusOpen, TicketTypeId = typeUI},
                                new Ticket() {Title = "Blog Ticket 11", Description = "Ticket details for blog ticket 11", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev,  TicketTypeId = typeMaintenance},
                                new Ticket() {Title = "Blog Ticket 12", Description = "Ticket details for blog ticket 12", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusNew,  TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Blog Ticket 13", Description = "Ticket details for blog ticket 13", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusOpen, TicketTypeId = typeRuntime},
                                new Ticket() {Title = "Blog Ticket 14", Description = "Ticket details for blog ticket 14", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusDev,  TicketTypeId = typeUI},
                                new Ticket() {Title = "Blog Ticket 15", Description = "Ticket details for blog ticket 15", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew,  TicketTypeId = typeMaintenance},
                                new Ticket() {Title = "Blog Ticket 16", Description = "Ticket details for blog ticket 16", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusOpen, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Blog Ticket 17", Description = "Ticket details for blog ticket 17", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev,  TicketTypeId = typeNewDev},
                                //BUGTRACKER                                                                                                                         
                                new Ticket() {Title = "Bug Tracker Ticket 1", Description = "Ticket details for Bug Tracker ticket 1", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 2", Description = "Ticket details for Bug Tracker ticket 2", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 3", Description = "Ticket details for Bug Tracker ticket 3", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 4", Description = "Ticket details for Bug Tracker ticket 4", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 5", Description = "Ticket details for Bug Tracker ticket 5", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 6", Description = "Ticket details for Bug Tracker ticket 6", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 7", Description = "Ticket details for Bug Tracker ticket 7", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 8", Description = "Ticket details for Bug Tracker ticket 8", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 9", Description = "Ticket details for Bug Tracker ticket 9", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 10", Description = "Ticket details for Bug Tracker ticket 10", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 11", Description = "Ticket details for Bug Tracker ticket 11", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 12", Description = "Ticket details for Bug Tracker ticket 12", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 13", Description = "Ticket details for Bug Tracker ticket 13", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 14", Description = "Ticket details for Bug Tracker ticket 14", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 15", Description = "Ticket details for Bug Tracker ticket 15", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 16", Description = "Ticket details for Bug Tracker ticket 16", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 17", Description = "Ticket details for Bug Tracker ticket 17", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 18", Description = "Ticket details for Bug Tracker ticket 18", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 19", Description = "Ticket details for Bug Tracker ticket 19", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 20", Description = "Ticket details for Bug Tracker ticket 20", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 21", Description = "Ticket details for Bug Tracker ticket 21", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 22", Description = "Ticket details for Bug Tracker ticket 22", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 23", Description = "Ticket details for Bug Tracker ticket 23", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 24", Description = "Ticket details for Bug Tracker ticket 24", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 25", Description = "Ticket details for Bug Tracker ticket 25", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 26", Description = "Ticket details for Bug Tracker ticket 26", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 27", Description = "Ticket details for Bug Tracker ticket 27", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 28", Description = "Ticket details for Bug Tracker ticket 28", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 29", Description = "Ticket details for Bug Tracker ticket 29", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 30", Description = "Ticket details for Bug Tracker ticket 30", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                };

                var dbTickets = context.Tickets.Select(c => c.Title).ToList();
                await context.Tickets.AddRangeAsync(tickets.Where(c => !dbTickets.Contains(c.Title)));
                context.SaveChanges();
            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Tickets.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }
    }
}

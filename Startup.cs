using InternalIssues.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InternalIssues.Data;
using InternalIssues.Models;
using InternalIssues.Utilities;
using InternalIssues.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Http;

namespace InternalIssues
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseNpgsql(DataUtility.GetConnectionString(Configuration)));
            services.AddDatabaseDeveloperPageExceptionFilter();
                        
            services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                    //.AddClaimsPrincipalFactory<AppUserClaimsPrincipalFactory>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews();
            services.AddRazorPages();


            //A new instance is provided everytime a request is made, however
            //an instance is reused when inside the same exact scope
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IHistoryService, HistoryService>();
            services.AddScoped<ITicketService, TicketService>();

            //Adds a NEW instance each and every single time this service is called
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<INotificationService, NotificationService>();

            //Setting up a configuration from what is inside appsettngs.json
            //Mapping a configuration entry to a class
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<IInviteService, InviteService>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=LandingPage}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}

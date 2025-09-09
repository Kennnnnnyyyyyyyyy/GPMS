using eTickets.Data;
using Gate_Pass_management.Data;
using Gate_Pass_management.Models;
using Gate_Pass_management.Infrastructure.Sqlite;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;            // ← keep this at the top
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Gate_Pass_management.Services;
using Gate_Pass_management.Hubs;

namespace Gate_Pass_management
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // SQLite database for local development with pragma interceptor
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnectionString"))
                       .AddInterceptors(new SqlitePragmaInterceptor()));
            // Identity (roles + 2FA ready)
            services
                .AddDefaultIdentity<AppUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false; // relax for dev
                    // Lockout policy
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                    // 2FA tokens
                    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            // Configure cookies explicitly
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
            
            // SignalR for real-time updates
            services.AddSignalR();
            
            // Authorization policies for Gate Pass
            services.AddAuthorization(options =>
            {
                options.AddPolicy("SecurityGuard", policy => policy.RequireRole("SecurityGuard", "Admin"));
                options.AddPolicy("Reception", policy => policy.RequireRole("Reception", "Admin"));
                options.AddPolicy("Employee", policy => policy.RequireRole("Employee", "Manager", "Admin"));
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            });
            
            // Existing services
            services.AddScoped<IGatePassService, GatePassService>();
            services.AddScoped<IVisitorMetricsService, VisitorMetricsService>();
            services.AddScoped<ISchedulingService, SchedulingService>();
            services.AddScoped<ISmsSender, TwilioSmsSender>();
            services.AddScoped<IAuditService, AuditService>();
            
            // Enhanced Gate Pass Workflow services
            services.AddScoped<IGatePassWorkflowService, GatePassWorkflowService>();
            services.AddScoped<IPdfGenerationService, PdfGenerationService>();
            services.AddScoped<IVisitorTrackingNotifier, VisitorTrackingNotifier>();
            
            services.AddHttpContextAccessor();
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            // Serve files from the project's /assets directory at the /assets URL path
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "assets")),
                RequestPath = "/assets"
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapControllers(); // Enable API controllers
                endpoints.MapHub<VisitorMetricsHub>("/hubs/visitorMetrics");
                endpoints.MapHub<VisitorTrackingHub>("/hubs/visitorTracking");
                endpoints.MapHub<SchedulerHub>("/hubs/scheduler");
            });

            // Seed the database with initial data
            AppDbInitializer.Seed(app);
        }
    }
}
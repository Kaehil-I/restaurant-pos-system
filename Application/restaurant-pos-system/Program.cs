using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using restaurant_pos_system.Models;

namespace restaurant_pos_system
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            var app = builder.Build();

            // Seed database
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var db = services.GetRequiredService<ApplicationDbContext>();

                    db.Database.Migrate();

                    // Create roles
                    var roles = new[] { "Manager", "Waiter", "Kitchen" };
                    foreach (var r in roles)
                    {
                        if (!roleManager.RoleExistsAsync(r).GetAwaiter().GetResult())
                            roleManager.CreateAsync(new IdentityRole(r)).GetAwaiter().GetResult();
                    }

                    // Create users with PINs
                    void EnsureUser(string userName, string email, string role, string pin)
                    {
                        var user = userManager.FindByNameAsync(userName).GetAwaiter().GetResult();
                        if (user == null)
                        {
                            user = new ApplicationUser
                            {
                                UserName = userName,
                                Email = email,
                                RoleType = role
                            };
                            user.PinHash = userManager.PasswordHasher.HashPassword(user, pin);
                            userManager.CreateAsync(user).GetAwaiter().GetResult();
                            userManager.AddToRoleAsync(user, role).GetAwaiter().GetResult();
                            logger.LogInformation("Created user {User} PIN={Pin}", userName, pin);
                        }
                    }

                    // PIN 1234 = Manager, PIN 2345 = Waiter, PIN 3456 = Kitchen
                    EnsureUser("manager", "manager@pos.com", "Manager", "1234");
                    EnsureUser("waiter", "waiter@pos.com", "Waiter", "2345");
                    EnsureUser("kitchen", "kitchen@pos.com", "Kitchen", "3456");
                }
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Seeding failed.");
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.MapRazorPages();
            app.Run();
        }
    }
}
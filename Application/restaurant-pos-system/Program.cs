using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // Register EF Core with an in-memory database for development/testing.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("AuthDb"));

            // Register Identity (with roles) and EF stores
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Configure cookie settings so unauthorized requests are redirected to the Account controller Login
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            var app = builder.Build();

            // Seed roles and example users (synchronous for Main; OK for dev seed)
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    logger.LogInformation("Starting DB seed.");

                    var roles = new[] { "Manager", "Waiter", "Kitchen" };
                    foreach (var r in roles)
                    {
                        var exists = roleManager.RoleExistsAsync(r).GetAwaiter().GetResult();
                        if (!exists)
                        {
                            var rr = roleManager.CreateAsync(new IdentityRole(r)).GetAwaiter().GetResult();
                            if (!rr.Succeeded)
                            {
                                logger.LogError("Failed to create role {Role}: {Errors}", r, string.Join(", ", rr.Errors));
                            }
                            else
                            {
                                logger.LogInformation("Created role {Role}", r);
                            }
                        }
                    }

                    // Helper to create a user if missing and set PinHash + role
                    void EnsureUser(string userName, string email, string role, string pin)
                    {
                        try
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

                                // Hash the PIN and set PinHash BEFORE creating the user so EF won't complain about missing required property.
                                var pinHash = userManager.PasswordHasher.HashPassword(user, pin);
                                user.PinHash = pinHash;

                                var createResult = userManager.CreateAsync(user).GetAwaiter().GetResult();
                                if (!createResult.Succeeded)
                                {
                                    logger.LogError("Failed creating user {User}: {Errors}", userName, string.Join(", ", createResult.Errors));
                                    return;
                                }

                                var addRoleResult = userManager.AddToRoleAsync(user, role).GetAwaiter().GetResult();
                                if (!addRoleResult.Succeeded)
                                {
                                    logger.LogError("Failed adding user {User} to role {Role}: {Errors}", userName, role, string.Join(", ", addRoleResult.Errors));
                                }
                                else
                                {
                                    logger.LogInformation("Created user {User} with role {Role}", userName, role);
                                }
                            }
                        }
                        catch (Exception userEx)
                        {
                            logger.LogError(userEx, "Failed creating or updating user {User}", userName);
                        }
                    }

                    EnsureUser("manager", "manager@example.com", "Manager", "1234");
                    EnsureUser("waiter", "waiter@example.com", "Waiter", "2345");
                    EnsureUser("kitchen", "kitchen@example.com", "Kitchen", "3456");

                    logger.LogInformation("DB seed completed.");
                }
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An exception occurred while seeding the database.");
                // Do not rethrow so you can start the host and inspect logs in the browser/Output window.
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Authentication must be enabled before authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Default route changed to start at the login screen
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            // Map Razor Pages (project contains Razor Pages)
            app.MapRazorPages();

            app.Run();
        }
    }
}

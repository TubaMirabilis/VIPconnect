using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.Models;
using System;
using System.Threading.Tasks;

namespace ProjectX.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw="")
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                //Making some roles for users:
                await EnsureRole(serviceProvider, "Administrator");
                await EnsureRole(serviceProvider, "Moderator");
                await EnsureRole(serviceProvider, "Banned");
                //Make the supreme Admin account if it's not already configured:
                var adminID = await EnsureUser(serviceProvider, testUserPw, "Admin");
                await AssignRole(serviceProvider, adminID, "Administrator");
            }
        }
        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string userName)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = "admin@theblink.network",
                    EmailConfirmed = true,
                    DateOfBirth = new DateTime(1994, 11, 18),
                    SightCategory = "Light Perception",
                    EmploymentStatus = "Working full-time",
                    JobTitle = "Administrator",
                    Company = "Blink",
                    Industry = "Webmastering",
                    WorkingSince = DateTime.UtcNow,
                    IsParent = "No",
                    WorkRoles = "Looking after the Blink website and Blink's presence on other platforms",
                    Strengths = "I have relentless determination, excellent time-management skills and a huge vocabulary!",
                    Joined = DateTime.UtcNow,
                    Biog = "My team of moderators and I are always willing to answer questions from members.  Use the contact form to get our attention."
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }
            return user.Id;
        }
        private static async Task EnsureRole(IServiceProvider serviceProvider, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        private static async Task<IdentityResult> AssignRole(IServiceProvider serviceProvider,
                                                              string uid, string role)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByIdAsync(uid);
            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }
            IdentityResult IR = await userManager.AddToRoleAsync(user, role);
            return IR;
        }
    }
}
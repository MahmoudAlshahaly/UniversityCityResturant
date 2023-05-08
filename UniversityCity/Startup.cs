using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;
using System.Data.Entity.Validation;
using UniversityCity.Models;

[assembly: OwinStartupAttribute(typeof(UniversityCity.Startup))]
namespace UniversityCity
{
    public partial class Startup
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
           CreateDefaultRoleAndUsers();
        }
        public void CreateDefaultRoleAndUsers()
        {
           
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                IdentityRole role = new IdentityRole();
            if (!roleManager.RoleExists("Admins"))
            {
                role.Name = "Admins";
                roleManager.Create(role);

                ApplicationUser user = new ApplicationUser();
                user.UserName = "mahmoud";
                user.Email = "anaengineer34@gmail.com";
                user.UserImage = "ahmed.jpg";
                user.UserBirthDay = DateTime.Now;
                user.UserFaculty = "علوم";
                user.UserType = "Admins";
                user.UserID = "29701101700515";
                var Check = userManager.Create(user, "1111111");
                if (Check.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admins");
                }

            }
            
        }
    }
}

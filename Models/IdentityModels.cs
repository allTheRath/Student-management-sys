using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Student_management_.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime? BirthDate { get; set; }
        public bool? IsOnVisa { get; set; }
        public PersonType? Type { get; set; }
        public virtual ICollection<ApplicationUserCourse> Courses { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    public enum PersonType
    {
        Admin,
        Teacher,
        Student
    }
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int Credits { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual ICollection<ApplicationUserCourse> ApplicationUsers { get; set; }
    }

    public class ApplicationUserCourse
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public DateTime DateJoined { get; set; }
    }

    public class WaitingList
    {
        [ForeignKey("Course")]
        public int Id { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<ApplicationUser> Students { get; set; }
        //we need a class to represent waiting lists,
    }

    public class ChangedUserIds
    {
        public int Id { get; set; }
        public DateTime EventTime = DateTime.Now;
        public DatabaseOperation Event { get; set; }
        public int LogingId { get; set; }
        public virtual Loging Loging { get; set; }
    }

    public class Loging
    {
        //and another one to represent a Log, where we register all the events like adding, deleting, assigning...etc
        public int Id { get; set; }
        public string UserId;
        public ICollection<ChangedUserIds> Changes { get; set; }
    }

    public enum DatabaseOperation
    {
        add,
        delete,
        assign,
        edit
    }

    public class RoleAuthManagement
    {
        private protected UserManager<IdentityUser> UserManager { get; set; }
        private protected RoleManager<IdentityRole> RoleManager { get; set; }

        public RoleAuthManagement()
        {
            UserManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>());
        }

        public bool IsUserInRole(string userId, string roleName)
        {
            bool result = false;
            result = UserManager.IsInRole(userId, roleName);
            return result;
        }

        public ICollection<string> GetAllUserRoles(string userId)
        {
            return UserManager.GetRoles(userId);
        }

        public ICollection<string> AllUsersOfRole(string roleName)
        {
            var users = RoleManager.FindByName(roleName).Users;
            var userIds = new List<string>();
            foreach (var u in users)
            {
                userIds.Add(u.UserId);
            }

            return userIds;
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            var result = false;
            if(RoleManager.RoleExists(roleName))
            {
               result = UserManager.AddToRole(userId, roleName).Succeeded;
            }
            return result;
        }

        public bool IsRoleExist(string roleName)
        {
            var result = false;
            result = RoleManager.RoleExists(roleName);
            return result;
        }

        public bool CreateRole(string roleName)
        {
            var result = false;
            result = RoleManager.Create(new IdentityRole(roleName)).Succeeded;
            return result;
        }

        public bool DeleteRole(string roleName)
        {
            var result = false;
            result = RoleManager.Delete(new IdentityRole(roleName)).Succeeded;
            return result;
        }

        public bool DeleteRoleFromUser(string userId, string roleName)
        {
            var result = false;
            result = UserManager.RemoveFromRole(userId, roleName).Succeeded;
            return result;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Loging> logings { get; set; }
        public DbSet<ChangedUserIds> ChangedUserIds { get; set; }
        public DbSet<ApplicationUserCourse> ApplicationUserCourses { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
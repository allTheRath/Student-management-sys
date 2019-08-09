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
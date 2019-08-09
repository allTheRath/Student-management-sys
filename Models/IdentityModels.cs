using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
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

    public class Log
    {
        //and another one to represent a Log, where we register all the events like adding, deleting, assigning...etc
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime EventTime = DateTime.Now;
        public DatabaseOperation Event { get; set; }
        public string ChangesMadeToUserId { get; set; }
        public TableName ChangeToTable { get; set; }
    }

    public enum TableName
    {
        Course,
        Teacher,
        Student,
        Admin
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
            if (RoleManager.RoleExists(roleName))
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

    public class DatabaseManupulation
    {
        private protected ApplicationDbContext db = new ApplicationDbContext();

        // methods for Course
        public ICollection<ApplicationUser> InstructorCourses()
        {
            var result = db.Users.Where(x => x.Type == PersonType.Teacher).ToList();

            return result;
        }

        public ICollection<Course> AllCourses()
        {
            var result = db.Courses.ToList();
            return result;
        }

        public Course RegisterForCourse(string courseName)
        {
            var result = db.Courses.FirstOrDefault(x => x.Name == courseName);
            return result;
        }

        public ICollection<Course> StudentCourses(string userId)
        {
            var result = db.ApplicationUserCourses.Where(x => x.ApplicationUserId == userId).Select(x => x.Course).ToList();

            return result;
        }


        public bool AddCourse(Course course)
        {
            try
            {
                db.Courses.Add(course);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool UpdateCourse()
        {
            try
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public ActionResult DeleteCourse()
        //{
        //    return View();
        //}

        //public ActionResult AddStudentToACourse()
        //{
        //    return View();
        //}

        ////public ActionResult AddStudentsTOCourse()
        ////{
        ////    return View();
        ////}

        //public ActionResult AddInstructorToCourse()
        //{
        //    return View();
        //}

        //public ActionResult RemoveStudentFromCourse()
        //{
        //    return View();
        //}

        //public ActionResult RemoveInstructorFromCourse()
        //{
        //    return View();
        //}

        //public ActionResult MaxCapaCityCourses()
        //{
        //    return View();
        //}

        //public ActionResult AllStudentsOfACourse()
        //{

        //    return View();
        //}


    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Log> Logs { get; set; }
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
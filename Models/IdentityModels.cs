using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Providers.Entities;
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
        edit,
        retrive
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

        private protected bool UpdateLog(string userID, DatabaseOperation operation, TableName tableName, string ChangesMadeToUser)
        {
            bool result = false;
            try
            {

                Log log = new Log();
                log.UserId = userID;
                log.Event = operation;
                log.ChangesMadeToUserId = ChangesMadeToUser;
                log.ChangeToTable = tableName;
                var length = db.Logs.Count();
                db.Logs.Add(log);
                db.SaveChanges();
                if (length + 1 != db.Logs.Count())
                {
                    throw new Exception();
                }
                result = true;
                return result;
            }
            catch (Exception)
            {
                return result;
            }

        }

        // methods for Course
        public ICollection<Course> InstructorCourses(string userId, string instructorId)
        {
            try
            {
                var log = UpdateLog(userId, DatabaseOperation.retrive, TableName.Teacher, instructorId);
                if (!log)
                {
                    throw new Exception();
                }
                var result = db.Users.FirstOrDefault(x => x.Id == instructorId).Courses.Select(x => x.CourseId).ToList();
                if (result == null || result.Count == 0)
                {
                    throw new Exception();
                }

                return db.Courses.Where(x => result.Contains(x.Id)).ToList();
            }
            catch (Exception)
            {
                var result = new List<Course>();
                return result;
            }

        }

        public ICollection<Course> AllCourses(string userId)
        {

            try
            {
                var log = UpdateLog(userId, DatabaseOperation.retrive, TableName.Course, "All");

                if (!log)
                {
                    throw new Exception();
                }

                var result = db.Courses.ToList();
                if (result == null || result.Count == 0)
                {
                    throw new Exception();
                }
                return result;
            }
            catch (Exception)
            {
                var result = new List<Course>();
                return result;
            }

        }

        public bool RegisterForCourse(string userId, string courseName, string studentId)
        {

            try
            {
                var log = UpdateLog(userId, DatabaseOperation.add, TableName.Course, courseName);

                if (!log)
                {
                    throw new Exception();
                }

                var student = db.Users.Find(studentId);
                var result = db.Courses.FirstOrDefault(x => x.Name == courseName);
                if (result == null || student == null)
                {
                    throw new Exception();
                }
                result.Capacity--;
                ApplicationUserCourse application = new ApplicationUserCourse();
                application.CourseId = result.Id;
                application.Course = result;
                application.ApplicationUserId = student.Id;
                application.ApplicationUser = student;
                application.DateJoined = DateTime.Now;
                db.ApplicationUserCourses.Add(application);
                db.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                return false;

            }
        }

        public ICollection<Course> StudentCourses(string userId, string studentId)
        {
            try
            {
                var log = UpdateLog(userId, DatabaseOperation.retrive, TableName.Course, studentId);

                if (!log)
                {
                    throw new Exception();
                }


                var result = db.ApplicationUserCourses.Where(x => x.ApplicationUserId == userId).Select(x => x.Course).ToList();
                if (result == null || result.Count == 0)
                {
                    throw new Exception();
                }
                return result;
            }
            catch (Exception)
            {
                var result = new List<Course>();

                return result;
            }

        }


        public bool AddCourse(string userId, Course course)
        {

            try
            {
                var log = UpdateLog(userId, DatabaseOperation.add, TableName.Course, course.Name);

                if (!log)
                {
                    throw new Exception();
                }

                var length = db.Courses.Count();
                db.Courses.Add(course);
                db.SaveChanges();
                if (length + 1 != db.Courses.Count())
                {
                    throw new Exception();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool UpdateCourse(string userId, Course course)
        {

            try
            {
                var log = UpdateLog(userId, DatabaseOperation.edit, TableName.Course, course.Name);

                if (!log)
                {
                    throw new Exception();
                }

                Course dbCourse = db.Courses.FirstOrDefault(x => x.Name == course.Name);
                if (dbCourse == null)
                {
                    throw new Exception();
                }
                dbCourse.Capacity = course.Capacity;
                dbCourse.Credits = course.Credits;
                dbCourse.EndDate = course.EndDate;
                dbCourse.StartDate = course.StartDate;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteCourse(string userId, string courseName)
        {
            bool result = false;

            try
            {
                var log = UpdateLog(userId, DatabaseOperation.delete, TableName.Course, courseName);

                if (!log)
                {
                    throw new Exception();
                }

                Course course = db.Courses.FirstOrDefault(x => x.Name == courseName);
                var length = db.Courses.Count();
                db.Courses.Remove(course);
                db.SaveChanges();
                if (length + 1 != db.Courses.Count())
                {
                    throw new Exception();
                }
                result = true;
                return result;

            }
            catch (Exception)
            {
                return result;
            }

        }

        public bool AddStudentToACourse(string userId, string studentId, string courseName)
        {
            bool result = false;
            try
            {
                var log = UpdateLog(userId, DatabaseOperation.add, TableName.Course, courseName);

                if (!log)
                {
                    throw new Exception();
                }

                var student = db.Users.Find(studentId);
                var course = db.Courses.FirstOrDefault(x => x.Name == courseName);
                if(student == null || course == null)
                {
                    throw new Exception();
                }

                var length = db.ApplicationUserCourses.Count();

                ApplicationUserCourse applicationUserCourse = new ApplicationUserCourse();
                applicationUserCourse.ApplicationUserId = student.Id;
                applicationUserCourse.CourseId = course.Id;
                applicationUserCourse.Course = course;
                applicationUserCourse.ApplicationUser = student;
                applicationUserCourse.DateJoined = DateTime.Now;
                course.ApplicationUsers.Add(applicationUserCourse);
                db.SaveChanges();

                if(length + 1 == db.ApplicationUserCourses.Count())
                {
                    result = true;
                }
                return result;
            }
            catch (Exception)
            {
                return result;
            }


        }

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
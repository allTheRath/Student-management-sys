using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Student_management_.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult AddRoleOnCreation(string role)
        {
            //var id = User.Identity.GetUserId();
            //Admin ad = new Admin();
            //var result = ad.AddUserToRole(id, role);
            //string controller = role + "s";
            //return RedirectToAction("Create", controller);
            return View();
        }
        public ActionResult InstructorCourses()
        {
            return View();
        }

        public ActionResult AllCourses()
        {
            return View();
        }

        public ActionResult RegisterForCourse()
        {
            return View();
        }

        public ActionResult StudentCources()
        {
            return View();
        }


        public ActionResult AddCourse()
        {
            return View();
        }

        public ActionResult UpdateCourse()
        {
            return View();
        }

        public ActionResult DeleteCourse()
        {
            return View();
        }

        public ActionResult AddStudentToACourse()
        {
            return View();
        }

        //public ActionResult AddStudentsTOCourse()
        //{
        //    return View();
        //}

        public ActionResult AddInstructorToCourse()
        {
            return View();
        }

        public ActionResult RemoveStudentFromCourse()
        {
            return View();
        }

        public ActionResult RemoveInstructorFromCourse()
        {
            return View();
        }

        public ActionResult MaxCapaCityCourses()
        {
            return View();
        }

        public ActionResult AllStudentsOfACourse()
        {

            return View();
        }


    }
}
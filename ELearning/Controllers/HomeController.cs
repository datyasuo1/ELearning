using BELibrary.Core.Entity;
using BELibrary.Entity;
using BELibrary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BELibrary.Core.Utils;
using BELibrary.DbContext;

namespace ELearning.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var unitOfWork = new UnitOfWork(new ELearningDbContext()))
            {
                var ourCourses = unitOfWork.Courses.Query(x => x.Status)
                    .OrderByDescending(x => x.Id)
                    .Take(5)
                    .ToList();

                ViewBag.OurCourses = ourCourses;

                var latestPosts = unitOfWork.Articles.Query(x => !x.IsDelete)
                    .OrderByDescending(x => x.ModifiedDate).Take(5).ToList();
                ViewBag.LatestPosts = latestPosts;

                var teacherUsers = unitOfWork.Account.Query(x => x.Status && x.RoleId == RoleKey.Teacher).Take(4).ToList();
                ViewBag.TeacherUsers = teacherUsers;

                var galleries = unitOfWork.Galleries.GetAll().OrderByDescending(x => x.ModifiedDate).Take(12).ToList();
                ViewBag.Galleries = galleries;
            }
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult ComingSoon()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult E404()
        {
            return View();
        }

        public ActionResult SetLanguage(string code)
        {
            var returnUrl = HttpContext.Request.UrlReferrer.ToString() ?? "/";
            if (LangCode.GetList().Any(x => x == code))
            {
                var cookieLang = Request.Cookies[CookiesKey.LangCode];
                if (cookieLang == null)
                {
                    HttpCookie newLangCookie = new HttpCookie(CookiesKey.LangCode)
                    {
                        Value = code,
                        Expires = DateTime.Now.AddDays(30)
                    };
                    HttpContext.Response.Cookies.Add(newLangCookie);
                }
                else
                {
                    cookieLang.Value = code;
                    cookieLang.Expires = DateTime.Now.AddDays(30);
                    HttpContext.Response.Cookies.Add(cookieLang);
                }
            }
            return Redirect(returnUrl);
        }
    }
}
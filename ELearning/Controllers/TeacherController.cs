using BELibrary.Core.Entity;
using BELibrary.DbContext;
using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;
using BELibrary.Core.Utils;
using BELibrary.Entity;

namespace ELearning.Controllers
{
    public class TeacherController : Controller
    {
        // GET: Article
        public ActionResult Index(string query, int? page)
        {
            if (query == "")
            {
                query = null;
            }

            ViewBag.QueryData = query;
            var pageNumber = (page ?? 1);
            const int pageSize = 8;

            using (var workScope = new UnitOfWork(new ELearningDbContext()))
            {
                var listData = workScope.Account.Query(x => x.Status && x.RoleId == RoleKey.Teacher);

                if (!string.IsNullOrEmpty(query))
                    listData = listData.Where(x => x.FullName.ToLower().Contains(query.Trim().ToLower()));

                var arts = listData.ToList();
                ViewBag.Total = arts.Count();

                return View(arts.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult Detail(string id)
        {
            using (var workScope = new UnitOfWork(new ELearningDbContext()))
            {
                var acUser = workScope.Account.FirstOrDefault(x => x.Username == id && x.Status);
                var teacherProfile = workScope.TeacherProfiles.FirstOrDefault(x => x.Username == acUser.Username) ?? new TeacherProfile();

                if (acUser != null)
                {
                    ViewBag.TeacherProfile = teacherProfile;
                    return View(acUser);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }
    }
}
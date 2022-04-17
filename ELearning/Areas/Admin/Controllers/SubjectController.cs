using BELibrary.Core.Entity;
using BELibrary.Entity;
using System;
using System.Linq;
using System.Web.Mvc;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Utils;
using ELearning.Areas.Admin.Authorization;

namespace ELearning.Areas.Admin.Controllers
{
    [Permission(Role = RoleKey.Admin)]
    public class SubjectController : BaseController
    {
        private readonly string keyElement = "Môn học";

        public ActionResult Index(int? courseId)
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = keyElement;

            ViewBag.BaseURL = "/admin/subject";

            using (var unitOfWork = new UnitOfWork(new ELearningDbContext()))
            {
                ViewBag.Courses = new SelectList(unitOfWork.Courses.GetAll().Select(x => new
                {
                    id = x.Id,
                    x.Name
                }), "Id", "Name");

                var listData = unitOfWork.Subjects.GetAll();

                if (courseId.HasValue)
                {
                    listData = listData.Where(x => x.CourseId == courseId.GetValueOrDefault()).OrderByDescending(x => x.ModifiedDate);
                    return View(listData.ToList());
                }
                return View(listData.OrderByDescending(x => x.ModifiedDate).ToList());
            }
        }

        public ActionResult Create()
        {
            ViewBag.key = "Thêm mới";
            ViewBag.Element = keyElement;

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1));

            ViewBag.isEdit = false;

            using (var unitOfWork = new UnitOfWork(new ELearningDbContext()))
            {
                var courses = unitOfWork.Courses.Query(x => x.Status).ToList();
                ViewBag.Courses = new SelectList(courses, "Id", "Name");
            }

            return View();
        }

        public ActionResult Update(int id)
        {
            ViewBag.isEdit = true;
            ViewBag.Feature = "Cập nhật";
            ViewBag.Element = keyElement;

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1));

            ViewBag.Element = keyElement;
            using (var unitOfWork = new UnitOfWork(new ELearningDbContext()))
            {
                var article = unitOfWork.Subjects.FirstOrDefault(x => x.Id == id);

                var courses = unitOfWork.Courses.Query(x => x.Status).ToList();
                ViewBag.Courses = new SelectList(courses, "Id", "Name");

                return View("Create", article);
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(Subject input, bool isEdit)
        {
            try
            {
                if (isEdit) //update
                {
                    using (var unitOfWork = new UnitOfWork(new ELearningDbContext()))
                    {
                        var elm = unitOfWork.Subjects.Get(input.Id);
                        if (elm != null) //update
                        {
                            input.CreatedBy = elm.CreatedBy;
                            input.CreatedDate = elm.CreatedDate;
                            elm = input;
                            elm.ModifiedBy = GetCurrentUser().FullName;
                            elm.ModifiedDate = DateTime.Now;
                            unitOfWork.Subjects.Put(elm, elm.Id);
                            unitOfWork.Complete();
                            return Json(new { status = true, mess = "Cập nhập thành công" });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + keyElement });
                        }
                    }
                }
                else
                {
                    using (var unitOfWork = new UnitOfWork(new ELearningDbContext()))
                    {
                        input.CreatedBy = GetCurrentUser().FullName;
                        input.CreatedDate = DateTime.Now;
                        input.ModifiedBy = GetCurrentUser().FullName;
                        input.ModifiedDate = DateTime.Now;
                        input.NumberStudent = 0;

                        unitOfWork.Subjects.Add(input);

                        unitOfWork.Complete();
                        return Json(new { status = true, mess = "Thêm thành công " + keyElement });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Del(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new ELearningDbContext()))
                {
                    var elm = unitOfWork.Subjects.Get(id);
                    if (elm != null) //update
                    {
                        unitOfWork.Subjects.Remove(elm);
                        unitOfWork.Complete();
                        return Json(new { status = true, mess = "Xóa thành công " + keyElement });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " + keyElement });
                    }
                }
            }
            catch
            {
                return Json(new { status = false, mess = "Thất bại" });
            }
        }
    }
}
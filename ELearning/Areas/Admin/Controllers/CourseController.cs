using BELibrary.Core.Entity;
using BELibrary.Entity;
using BELibrary.Utils;
using ELearning.Areas.Admin.Authorization;
using System;
using System.Linq;
using System.Web.Mvc;
using BELibrary.Core.Utils;
using BELibrary.DbContext;

namespace ELearning.Areas.Admin.Controllers
{
    [Permission(Role = RoleKey.Admin)]
    public class CourseController : BaseController
    {
        private string keyElement = "Khóa học";

        public ActionResult Index()
        {
            ViewBag.Element = keyElement;

            ViewBag.Feature = "Danh sách";

            ViewBag.BaseURL = "/admin/course";

            using (var unitOfWork = new UnitOfWork(new ELearningDbContext()))
            {
                var listData = unitOfWork.Courses.GetAll().OrderByDescending(x => x.ModifiedDate).ToList();
                return View(listData);
            }
        }

        public ActionResult Create()
        {
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = keyElement;

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1));

            ViewBag.isEdit = false;

            ViewBag.LangCode = new SelectList(LangCode.GetDic(), "Key", "Value");

            return View();
        }

        public ActionResult Update(int id)
        {
            ViewBag.isEdit = true;
            ViewBag.Feature = "Cập nhật";
            ViewBag.Element = keyElement;

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1));

            using (var unitOfWork = new UnitOfWork(new ELearningDbContext()))
            {
                ViewBag.LangCode = new SelectList(LangCode.GetDic(), "Key", "Value");

                var article = unitOfWork.Courses.FirstOrDefault(x => x.Id == id);
                return View("Create", article);
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(Course input, bool isEdit)
        {
            try
            {
                if (isEdit) //update
                {
                    using (var unitOfWork = new UnitOfWork(new ELearningDbContext()))
                    {
                        var elm = unitOfWork.Courses.Get(input.Id);
                        if (elm != null) //update
                        {
                            input.CreatedBy = elm.CreatedBy;
                            input.CreatedDate = elm.CreatedDate;
                            elm = input;
                            elm.ModifiedBy = GetCurrentUser().FullName;
                            elm.ModifiedDate = DateTime.Now;
                            unitOfWork.Courses.Put(elm, elm.Id);
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

                        unitOfWork.Courses.Add(input);

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
                    var elm = unitOfWork.Courses.Get(id);
                    if (elm != null) //update
                    {
                        unitOfWork.Courses.Remove(elm);
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
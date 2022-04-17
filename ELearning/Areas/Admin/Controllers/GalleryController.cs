using BELibrary.Core.Entity;
using BELibrary.Utils;
using ELearning.Areas.Admin.Authorization;
using PagedList;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;

namespace ELearning.Areas.Admin.Controllers
{
    [Permission(Role = RoleKey.Admin)]
    public class GalleryController : BaseController
    {
        private const string keyElement = "Bộ sưu tập";

        // GET: Admin/Article
        public ActionResult Index()
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = keyElement;

            ViewBag.BaseURL = "/admin/gallery";

            using (var workScope = new UnitOfWork(new ELearningDbContext()))
            {
                var galleries = workScope.Galleries.GetAll().ToList();
                return View(galleries);
            }
        }

        public ActionResult Create()
        {
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = keyElement;

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1));

            ViewBag.isEdit = false;

            ViewBag.GalleryType = new SelectList(GalleryType.GetDic(), "Key", "Value");

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
                ViewBag.GalleryType = new SelectList(GalleryType.GetDic(), "Key", "Value");

                var article = unitOfWork.Galleries.FirstOrDefault(x => x.Id == id);
                return View("Create", article);
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(Gallery input, bool isEdit)
        {
            try
            {
                if (isEdit) //update
                {
                    using (var unitOfWork = new UnitOfWork(new ELearningDbContext()))
                    {
                        var elm = unitOfWork.Galleries.Get(input.Id);
                        if (elm != null) //update
                        {
                            input.CreatedBy = elm.CreatedBy;
                            input.CreatedDate = elm.CreatedDate;
                            elm = input;
                            elm.ModifiedBy = GetCurrentUser().FullName;
                            elm.ModifiedDate = DateTime.Now;
                            unitOfWork.Galleries.Put(elm, elm.Id);
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

                        unitOfWork.Galleries.Add(input);

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
                    var elm = unitOfWork.Galleries.Get(id);
                    if (elm != null) //update
                    {
                        unitOfWork.Galleries.Remove(elm);
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
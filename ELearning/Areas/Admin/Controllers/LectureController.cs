using BELibrary.Core.Entity;
using BELibrary.Entity;
using BELibrary.Utils;
using System;
using System.Linq;
using System.Web.Mvc;
using BELibrary.Core.Utils;
using BELibrary.DbContext;

namespace ELearning.Areas.Admin.Controllers
{
    public class LectureController : BaseController
    {
        private readonly string keyElement = "Bài học";

        public ActionResult Index(int? subjectId)
        {
            ViewBag.Element = keyElement;
            ViewBag.BaseURL = "/admin/lecture";
            ViewBag.Feature = "Danh sách";

            var user = GetCurrentUser();
            using (var workScope = new UnitOfWork(new ELearningDbContext()))
            {
                ViewBag.Subjects = new SelectList(workScope.Subjects.GetAll().Select(x => new
                {
                    id = x.Id,
                    x.Name
                }), "Id", "Name");

                switch (user.RoleId)
                {
                    case RoleKey.Teacher:
                        {
                            //Lấy danh sách môn học đc giao của giáo viên
                            var subjectAssign =
                                workScope.TeacherSubjects.Query(x => x.Username.ToLower() == user.Username.ToLower());

                            //Join bảng môn học và môn học được giao của giáo viên đang đăng nhập
                            var subjects = workScope.Subjects.GetAll().Join(
                                subjectAssign,
                                sj => sj.Id,
                                sja => sja.SubjectId,
                                (sj, sja) => new { sj }).Select(x => x.sj).Where(x => x.Status).ToList();

                            var listData = workScope.Lectures.Include(x => x.Subject).Join(
                                subjects,
                                lt => lt.SubjectId,
                                sj => sj.Id,
                                (lt, sj) => new { lt }).Select(x => x.lt);

                            if (subjectId.HasValue)
                            {
                                listData = listData.Where(x => x.SubjectId == subjectId);
                                return View(listData.ToList());
                            }

                            return View(listData.ToList());
                        }
                    case RoleKey.Admin:
                        {
                            var listData = workScope.Lectures.Include(x => x.Subject);

                            if (subjectId.HasValue)
                            {
                                listData = listData.Where(x => x.SubjectId == subjectId);
                                return View(listData.ToList());
                            }
                            return View(listData.ToList());
                        }
                    default:
                        return RedirectToAction("E401", "Dashboard");
                }
            }
        }

        public ActionResult Create()
        {
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = keyElement;

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1));

            ViewBag.isEdit = false;

            using (var workScope = new UnitOfWork(new ELearningDbContext()))
            {
                var user = GetCurrentUser();

                switch (user.RoleId)
                {
                    case RoleKey.Teacher:
                        {
                            //Lấy danh sách môn học đc giao của giáo viên
                            var subjectAssign =
                                workScope.TeacherSubjects.Query(x => x.Username.ToLower() == user.Username.ToLower());

                            //Join bảng môn học và môn học được giao của giáo viên đang đăng nhập
                            var subject = workScope.Subjects.GetAll().Join(
                                subjectAssign,
                                sj => sj.Id,
                                sja => sja.SubjectId,
                                (sj, sja) => new { sj }).Select(x => x.sj).Where(x => x.Status).ToList();

                            ViewBag.Subject = new SelectList(subject, "Id", "Name");
                            break;
                        }
                    case RoleKey.Admin:
                        {
                            var subject = workScope.Subjects.Query(x => x.Status).ToList();

                            ViewBag.Subject = new SelectList(subject, "Id", "Name");
                            break;
                        }
                    default:
                        return RedirectToAction("E401", "Dashboard");
                }
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

            using (var workScope = new UnitOfWork(new ELearningDbContext()))
            {
                var lecture = workScope.Lectures.FirstOrDefault(x => x.Id == id);
                var user = GetCurrentUser();
                switch (user.RoleId)
                {
                    case RoleKey.Teacher:
                        {
                            var subjectAssign =
                                workScope.TeacherSubjects.Query(x => x.Username.ToLower() == user.Username.ToLower());

                            var subjects = workScope.Subjects.GetAll().Join(
                                subjectAssign,
                                sj => sj.Id,
                                sja => sja.SubjectId,
                                (sj, sja) => new { sj }).Select(x => x.sj).Where(x => x.Status).ToList();

                            if (subjects.Any(x => x.Id != lecture.SubjectId))
                            {
                                return RedirectToAction("E401", "Dashboard");
                            }

                            ViewBag.Subject = new SelectList(subjects, "Id", "Name");

                            break;
                        }
                    case RoleKey.Admin:
                        {
                            var subject = workScope.Subjects.Query(x => x.Status).ToList();

                            ViewBag.Subject = new SelectList(subject, "Id", "Name");
                            break;
                        }
                    default:
                        return RedirectToAction("E401", "Dashboard");
                }

                return View("Create", lecture);
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(Lecture input, bool isEdit)
        {
            var user = GetCurrentUser();
            try
            {
                if (isEdit) //update
                {
                    using (var workScope = new UnitOfWork(new ELearningDbContext()))
                    {
                        //Check quyền
                        var elm = workScope.Lectures.Get(input.Id);

                        switch (user.RoleId)
                        {
                            case RoleKey.Teacher:
                                {
                                    var subjectAssign =
                                        workScope.TeacherSubjects.Query(x => x.Username.ToLower() == user.Username.ToLower());

                                    var subjects = workScope.Subjects.GetAll().Join(
                                        subjectAssign,
                                        sj => sj.Id,
                                        sja => sja.SubjectId,
                                        (sj, sja) => new { sj }).Select(x => x.sj).Where(x => x.Status).ToList();

                                    if (subjects.Any(x => x.Id != elm.SubjectId))
                                    {
                                        return Json(new { status = false, mess = "Không có quyền " });
                                    }

                                    break;
                                }
                            case RoleKey.Admin:
                                {
                                    break;
                                }
                            default:
                                return Json(new { status = false, mess = "Không có quyền " });
                        }

                        if (elm != null) //update
                        {
                            input.CreatedBy = elm.CreatedBy;
                            input.CreatedDate = elm.CreatedDate;
                            elm = input;
                            elm.ModifiedBy = GetCurrentUser().FullName;
                            elm.ModifiedDate = DateTime.Now;
                            workScope.Lectures.Put(elm, elm.Id);
                            workScope.Complete();
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
                    using (var workScope = new UnitOfWork(new ELearningDbContext()))
                    {
                        input.CreatedBy = GetCurrentUser().FullName;
                        input.CreatedDate = DateTime.Now;
                        input.ModifiedBy = GetCurrentUser().FullName;
                        input.ModifiedDate = DateTime.Now;

                        switch (user.RoleId)
                        {
                            case RoleKey.Teacher:
                                {
                                    var subjectAssign =
                                        workScope.TeacherSubjects.Query(x => x.Username.ToLower() == user.Username.ToLower());

                                    var subjects = workScope.Subjects.GetAll().Join(
                                        subjectAssign,
                                        sj => sj.Id,
                                        sja => sja.SubjectId,
                                        (sj, sja) => new { sj }).Select(x => x.sj).Where(x => x.Status).ToList();

                                    if (subjects.Any(x => x.Id != input.SubjectId))
                                    {
                                        return Json(new { status = false, mess = "Không có quyền " });
                                    }
                                    break;
                                }
                            case RoleKey.Admin:
                                {
                                    break;
                                }
                            default:
                                return Json(new { status = false, mess = "Không có quyền " });
                        }

                        workScope.Lectures.Add(input);

                        workScope.Complete();
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
                using (var workScope = new UnitOfWork(new ELearningDbContext()))
                {
                    var elm = workScope.Lectures.Get(id);
                    if (elm != null) //update
                    {
                        workScope.Lectures.Remove(elm);
                        workScope.Complete();
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
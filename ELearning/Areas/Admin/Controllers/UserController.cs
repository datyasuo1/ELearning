using BELibrary.Core.Entity;
using BELibrary.Entity;
using BELibrary.Utils;
using System;
using System.Linq;
using System.Web.Mvc;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using ELearning.Areas.Admin.Authorization;

namespace ELearning.Areas.Admin.Controllers
{
    [Permission(Role = RoleKey.Admin)]
    public class UserController : BaseController
    {
        public ActionResult Index(int role)
        {
            if (!RoleKey.Any(role))
            {
                return Redirect("/admin");
            }

            ViewBag.Element = RoleKey.GetRoleText(role);
            ViewBag.Feature = "Danh sách";

            ViewBag.BaseURL = "/admin/user?role=" + role;

            ViewBag.Role = role;
            using (var workScope = new UnitOfWork(new ELearningDbContext()))
            {
                var listData = workScope.Account.GetAll().Where(x => x.RoleId == role).ToList();
                return View(listData);
            }
        }

        public ActionResult Create(int role)
        {
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = RoleKey.GetRoleText(role);

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1)) + "?role=" + role;

            ViewBag.isEdit = false;
            ViewBag.Role = role;
            ViewBag.Genders = new SelectList(GenderKey.GetDic(), "Key", "Value");
            ViewBag.Roles = new SelectList(RoleKey.GetDic(), "Key", "Value");

            return View();
        }

        public ActionResult Update(string username, int role)
        {
            ViewBag.isEdit = true;
            ViewBag.Role = role;
            ViewBag.Feature = "Cập nhật";

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1)) + "?role=" + role;

            ViewBag.Element = RoleKey.GetRoleText(role);
            ViewBag.Genders = new SelectList(GenderKey.GetDic(), "Key", "Value");
            ViewBag.Roles = new SelectList(RoleKey.GetDic(), "Key", "Value");

            using (var workScope = new UnitOfWork(new ELearningDbContext()))
            {
                var acc = workScope.Account.FirstOrDefault(x => x.Username == username);
                acc.Password = "";
                return View("Create", acc);
            }
        }

        public ActionResult TeacherProfile(string username, int role)
        {
            if (role != RoleKey.Teacher)
            {
                return Redirect(string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1)) + "?role=" + role);
            }

            ViewBag.isEdit = true;
            ViewBag.Role = role;
            ViewBag.Feature = "Cập nhật thông tin giáo viên";

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1)) + "?role=" + role;

            ViewBag.Element = RoleKey.GetRoleText(role);
            ViewBag.Genders = new SelectList(GenderKey.GetDic(), "Key", "Value");
            ViewBag.Roles = new SelectList(RoleKey.GetDic(), "Key", "Value");

            using (var workScope = new UnitOfWork(new ELearningDbContext()))
            {
                var acc = workScope.Account.FirstOrDefault(x => x.Username == username);

                var teacherProfile = workScope.TeacherProfiles.FirstOrDefault(x => x.Username == acc.Username) ?? new TeacherProfile
                {
                    Username = acc.Username,
                    FacebookLink = "https://www.facebook.com/",
                    TwitterLink = "https://twitter.com/",
                    SkypeLink = "live:",
                };

                return View("TeacherProfile", teacherProfile);
            }
        }

        [HttpPost]
        public JsonResult CreateOrEdit(User input, bool isEdit)
        {
            if (GetCurrentUser() != null && GetCurrentUser().RoleId == RoleKey.Admin)
            {
                try
                {
                    if (isEdit) //update
                    {
                        using (var workScope = new UnitOfWork(new ELearningDbContext()))
                        {
                            var elm = workScope.Account.FirstOrDefault(x => x.Username == input.Username);
                            if (elm != null) //update
                            {
                                //xu ly password

                                //var passwordFactory = input.Password + VariableExtensions.KeyCryptor;
                                //var passwordCrypto = CryptorEngine.Encrypt(passwordFactory, true);

                                //input.Password = passwordCrypto != elm.Password ? passwordCrypto : elm.Password;

                                input.Password = elm.Password;
                                elm = input;

                                workScope.Account.Put(elm, elm.Username);
                                workScope.Complete();
                                return Json(new { status = true, mess = "Cập nhập thành công" });
                            }
                            else
                            {
                                return Json(new { status = false, mess = "Không tồn tại " });
                            }
                        }
                    }
                    else
                    {
                        using (var workScope = new UnitOfWork(new ELearningDbContext()))
                        {
                            var passwordFactory = input.Password + VariableExtensions.KeyCryptor;
                            var passwordCrypto = CryptorEngine.Encrypt(passwordFactory, true);
                            input.Password = passwordCrypto;
                            workScope.Account.Add(input);
                            workScope.Complete();
                            return Json(new { status = true, mess = "Thêm thành công" });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
                }
            }
            else
            {
                return Json(new { status = false, mess = "Bạn không có quyền" });
            }
        }

        [HttpPost]
        public JsonResult CreateOrEditTeacherProfile(TeacherProfile input)
        {
            if (GetCurrentUser() != null && GetCurrentUser().RoleId == RoleKey.Admin)
            {
                try
                {
                    using (var workScope = new UnitOfWork(new ELearningDbContext()))
                    {
                        if (input.Id >= 0)
                        {
                            var elm = workScope.TeacherProfiles.FirstOrDefault(x => x.Id == input.Id);
                            if (elm != null) //update
                            {
                                elm = input;

                                workScope.TeacherProfiles.Put(elm, elm.Id);
                                workScope.Complete();
                                return Json(new { status = true, mess = "Cập nhập thành công" });
                            }
                        }
                        else
                        {
                            workScope.TeacherProfiles.Add(input);
                            workScope.Complete();
                            return Json(new { status = true, mess = "Thêm thành công" });
                        }
                        return Json(new { status = false, mess = "Có lỗi xảy ra" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
                }
            }
            else
            {
                return Json(new { status = false, mess = "Bạn không có quyền" });
            }
        }

        [HttpPost]
        public JsonResult Del(string username)
        {
            try
            {
                using (var workScope = new UnitOfWork(new ELearningDbContext()))
                {
                    var elm = workScope.Account.FirstOrDefault(x => x.Status && x.Username == username);
                    if (elm != null) //update
                    {
                        elm.Status = false;
                        workScope.Account.Put(elm, elm.Username);
                        workScope.Complete();
                        return Json(new { status = true, mess = "Xóa thành công " });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " });
                    }
                }
            }
            catch
            {
                return Json(new { status = false, mess = "Thất bại" });
            }
        }

        [HttpPost]
        public JsonResult ChangePassword(string username, string password, string rePassword)
        {
            try
            {
                var user = GetCurrentUser();

                if (user.RoleId != RoleKey.Admin)
                {
                    return Json(new { status = false, mess = "Bạn không có quyền" });
                }

                if (password != rePassword)
                {
                    return Json(new { status = false, mess = "Mật khẩu không giống nhau" });
                }

                using (var workScope = new UnitOfWork(new ELearningDbContext()))
                {
                    var elm = workScope.Account.FirstOrDefault(x => x.Username == username);
                    if (elm != null) //update
                    {
                        //xu ly password

                        var passwordFactory = password + VariableExtensions.KeyCryptor;
                        var passwordCrypto = CryptorEngine.Encrypt(passwordFactory, true);
                        elm.Password = passwordCrypto;

                        workScope.Account.Put(elm, elm.Username);
                        workScope.Complete();

                        return Json(new { status = true, mess = "Cập nhật mật khẩu thành công" });
                    }

                    return Json(new { status = false, mess = "User không tồn tại" });
                }
            }
            catch
            {
                return Json(new { status = false, mess = "Thất bại" });
            }
        }
    }
}
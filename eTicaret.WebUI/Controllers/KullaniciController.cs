using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using eTicaret.WebUI.App_Classes;
using eTicaret.WebUI.Models;

namespace eTicaret.WebUI.Controllers
{
    [Authorize]
    public class KullaniciController : Controller
    {
        // GET: Kullanici
        public ActionResult Index()
        {
            MembershipUserCollection users = Membership.GetAllUsers();
            ViewBag.Roller = Roles.GetAllRoles().ToList();
            return View(users);
        }

        [HttpPost]
        [Authorize(Roles="Admin")]
        public ActionResult RolAta(string KullaniciAdi, string RolAdi)
        {
            Roles.AddUserToRole(KullaniciAdi, RolAdi);
            return RedirectToAction("Index");
        }

        //[HttpPost]
        //public ActionResult UyeRolleri(string kullaniciAdi)
        //{
        //    List<string> roller=Roles.GetRolesForUser(kullaniciAdi).ToList();
        //    string rol = "";
        //    foreach (string r in roller)
        //    {
        //        rol += r + "-";
        //    }

        //    rol = rol.Remove(rol.Length - 1, 1);
        //    return rol;
        //}

        [HttpPost]
        public string UyeRolleri(string kullaniciAdi)
        {
            List<string> roller = Roles.GetRolesForUser(kullaniciAdi).ToList();
            string rol = "";
            foreach (string r in roller)
            {
                rol += r + "-";
            }
            rol = rol.Remove(rol.Length - 1, 1);
            return rol;
        }
        [AllowAnonymous]
        public ActionResult GirisYap()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Ekle(Kullanici k)
        {
            
            MembershipCreateStatus durum;
            Membership.CreateUser(k.KullaniciAdi, k.Parola, k.Email, k.GizliSoru, k.GizliCevap, true, out durum);
            string mesaj = "";
            switch (durum)
            {
                case MembershipCreateStatus.Success:
                    break;
                case MembershipCreateStatus.InvalidUserName:
                    mesaj += "Geçersiz kullanıcı adı";
                    break;
                case MembershipCreateStatus.InvalidPassword:
                    mesaj += "Geçersiz parola";
                    break;
                case MembershipCreateStatus.InvalidQuestion:
                    break;
                case MembershipCreateStatus.InvalidAnswer:
                    break;
                case MembershipCreateStatus.InvalidEmail:
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    mesaj += "Kullanılmış Kullanıcı Adı girildi";
                    break;
                case MembershipCreateStatus.DuplicateEmail:
                    mesaj += "Kullanılmış Mail adresi girildi";
                    break;
                case MembershipCreateStatus.UserRejected:
                    break;
                case MembershipCreateStatus.InvalidProviderUserKey:
                    mesaj += "geçersiz userkey";
                    break;
                case MembershipCreateStatus.DuplicateProviderUserKey:
                    break;
                case MembershipCreateStatus.ProviderError:
                    break;
                default:
                    break;
            }
     
            ViewBag.Mesaj = mesaj;
            if (durum==MembershipCreateStatus.Success)
                return RedirectToAction("Hesabim","Uye");
            else
                return View();
        }

        public ActionResult RolAta(string id)
        {
            ViewBag.Roller = Roles.GetAllRoles().ToList();
            return View(id);
        }

        
    }
}
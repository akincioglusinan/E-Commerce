using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eTicaret.WebUI.Controllers
{

    using App_Classes;
    using System.Web.Security;
    using eTicaret.WebUI.Models;

    [Authorize]
    public class UyeController : Controller
    {
        // GET: Uye
        [AllowAnonymous]
        public ActionResult GirisYap()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GirisYap(Kullanici k, string Hatirla)
        {

            aspnet_Membership uye = Context.Baglanti.aspnet_Membership.FirstOrDefault(x => x.Email == k.Email);
            if (uye == null)
            {
                ViewBag.Mesaj = "Email hatalı!";
                return View();
            }
            aspnet_Users uyes = Context.Baglanti.aspnet_Users.FirstOrDefault(x => x.UserId == uye.UserId);
            bool sonuc = Membership.ValidateUser(uyes.UserName, k.Parola);
            if (sonuc)
            {
                if (Hatirla == "on")
                    FormsAuthentication.RedirectFromLoginPage(uyes.UserName, true);
                else
                    FormsAuthentication.RedirectFromLoginPage(uyes.UserName, false);
                //Request.UrlReferrer.ToString()
                return RedirectToAction("Index","Home");
            }
            else
            {
                ViewBag.Mesaj = "Parola hatalı!";
                return View();
            }
        }


        public ActionResult CikisYap()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult ParolamiUnuttum()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ParolamiUnuttum(Kullanici k)
        {
            MembershipUser mu = Membership.GetUser(k.KullaniciAdi);

            //MembershipUser user = Membership.GetUser("username", false);
            //string resetPassword = user.ResetPassword();
            //user.ChangePassword(resetPassword, "new password to set");

            if (mu.PasswordQuestion == k.GizliSoru)
            {
                string respwd = mu.ResetPassword(k.GizliCevap);
                mu.ChangePassword(respwd, k.Parola);
                return RedirectToAction("GirisYap", "Uye");
            }
            else
            {
                ViewBag.Mesaj = "Girilen Gizli Soru veya Soru Cevabı yanlış! ";
                return View();
            }
        }

        public ActionResult Hesabim()
        {

            Musteri mst = Context.Baglanti.Musteris.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
            if (mst == null)
            {
                return RedirectToAction("HesapDuzenle");
            }
            else
                return View(mst);
        }

        public ActionResult HesapDuzenle()
        {
            aspnet_Users uye = Context.Baglanti.aspnet_Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            aspnet_Membership uyes = Context.Baglanti.aspnet_Membership.FirstOrDefault(x => x.UserId == uye.UserId);
            Musteri mu = Context.Baglanti.Musteris.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
            ViewBag.Mu = mu;
            return View(uyes);
        }

        [HttpPost]
        public ActionResult HesapDuzenle(Musteri m)
        {
            Musteri mu = Context.Baglanti.Musteris.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
            if (mu != null)
            {
                mu.KullaniciAdi = m.KullaniciAdi;
                mu.Id = m.Id;
                mu.Tckn = m.Tckn;
                mu.Adi = m.Adi;
                mu.Soyadi = m.Soyadi;
                mu.EMail = m.EMail;
                mu.Telefon = m.Telefon;
                Context.Baglanti.SaveChanges();
            }
            else
            {
                Context.Baglanti.Musteris.Add(m);
                Context.Baglanti.SaveChanges();
            }

            if (Request.UrlReferrer.LocalPath == "/Siparis/SiparisVer")
            {
                return RedirectToAction("OdemeYap", "Siparis");
            }
            else
            {
                return RedirectToAction("Hesabim");
            }

        }

        public ActionResult Adreslerim()
        {
            aspnet_Users uye = Context.Baglanti.aspnet_Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            List<MusteriAdre> ma = Context.Baglanti.MusteriAdres.Where(x => x.MusteriID == uye.UserId).ToList();

            if (ma == null)
            {
                return RedirectToAction("AdresEkle", "Uye");
            }
            else
            {
                return View(ma);
            }

        }

        public ActionResult AdresGetirWidget(int id)
        {
            MusteriAdre ma = Context.Baglanti.MusteriAdres.FirstOrDefault(x => x.Id == id);
            var Sepet = (Sepet)HttpContext.Session["AktifSepet"];
            Sepet.AdresID = ma.Id;
            return PartialView(ma);
        }

        public ActionResult AdresEkle()
        {
            aspnet_Users aktifuye = Context.Baglanti.aspnet_Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            return View(aktifuye);
        }

        [HttpPost]
        public ActionResult AdresEkle(MusteriAdre ma)
        {
            if (ma == null)
            {
                return RedirectToAction("AdresEkle");
            }
            else
            {
                Context.Baglanti.MusteriAdres.Add(ma);
                Context.Baglanti.SaveChanges();
            }
            if (Request.UrlReferrer.LocalPath == "/Siparis/SiparisVer")
            {
                return RedirectToAction("SiparisVer", "Siparis");
            }
            else
            {
                return RedirectToAction("Adreslerim");
            }
        }

        public ActionResult AdresSil(int id)
        {
            MusteriAdre ma = Context.Baglanti.MusteriAdres.FirstOrDefault(x => x.Id == id);
            Context.Baglanti.MusteriAdres.Remove(ma);
            Context.Baglanti.SaveChanges();
            return RedirectToAction("Adreslerim");
        }

        public ActionResult Siparislerim()
        {
            
            Musteri mu = Context.Baglanti.Musteris.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
            List<Sati> sat = Context.Baglanti.Satis.Where(x => x.MusteriID == mu.Id).ToList();
            List<SiparisDetay> sd = Context.Baglanti.SiparisDetays.ToList();
            List<Urun> urn = Context.Baglanti.Uruns.ToList();
            var satlist = from e in sat
                          join d in sd on e.Id equals d.SatisID into table1
                          from d in table1.ToList()
                          join i in urn on d.UrunID equals i.Id into table2
                          from i in table2.ToList()
                         
                          select new ViewModel
                          {
                              satis = e,
                              sdetay = d,
                              urunu = i,
                             
                          };
            //foreach (var item in sat)
            //{
            //    foreach (var x in sd)
            //    {
            //        if (item.Id == x.SatisID)
            //        {
            //               urn=urn.Concat(Context.Baglanti.Uruns.Where(p => p.Id==x.UrunID)).ToList();
            //        }
            //    }
            //}
            ViewBag.Sat = sat;
            ViewBag.urunler = urn;
            return View(satlist.OrderByDescending(x=>x.satis.SatisTarihi));
        }

        public ActionResult Yorumlarim()
        {

            Musteri mu = Context.Baglanti.Musteris.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
            List<Yorum> y = Context.Baglanti.Yorums.Where(x => x.MusteriID == mu.Id).ToList();
            return View(y);
        }
    }
}
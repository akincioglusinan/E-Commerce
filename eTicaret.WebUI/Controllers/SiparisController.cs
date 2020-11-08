using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eTicaret.WebUI.App_Classes;
using eTicaret.WebUI.Models;

namespace eTicaret.WebUI.Controllers
{
    public class SiparisController : Controller
    {
        // GET: Siparis
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SiparisVer()
        {
            if (User.Identity.IsAuthenticated)
            {
                aspnet_Users uye = Context.Baglanti.aspnet_Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                aspnet_Membership uyes = Context.Baglanti.aspnet_Membership.FirstOrDefault(x => x.UserId == uye.UserId);
                Musteri m = Context.Baglanti.Musteris.FirstOrDefault(x => x.Id == uye.UserId);
                ViewBag.M = m;
                ViewBag.MAdres = Context.Baglanti.MusteriAdres.Where(x => x.MusteriID == m.Id).ToList();
                ViewBag.Sepet = (Sepet)HttpContext.Session["AktifSepet"];
                return View(uyes);
            }
            else
            {
                return RedirectToAction("GirisYap", "Kullanici");
            }

        }
        
        //public PartialViewResult AdresSiparis(int? adid)
        //{
        //    var Sepet = (Sepet)HttpContext.Session["AktifSepet"];
        //    Sepet.AdresID = (int)adid;
        //    return PartialView();
        //}

        public ActionResult OdemeYap()
        {
            return View((Sepet)HttpContext.Session["AktifSepet"]);
        }

        [HttpPost]
        public ActionResult OdemeYap(int? kartno)
        {
            aspnet_Users uye = Context.Baglanti.aspnet_Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            aspnet_Membership uyes = Context.Baglanti.aspnet_Membership.FirstOrDefault(x => x.UserId == uye.UserId);
            Musteri m = Context.Baglanti.Musteris.FirstOrDefault(x => x.Id == uye.UserId);
            var sepet = (Sepet)HttpContext.Session["AktifSepet"];
            Sati s = new Sati();
            s.MusteriID = m.Id;
            s.ToplamTutar = sepet.ToplamTutar;
            s.SepetteMi = false;
            s.SiparisDurumID = 1;
            s.SatisTarihi = DateTime.Now;
            s.MusteriAdresID = sepet.AdresID;
            Context.Baglanti.Satis.Add(s);
            Context.Baglanti.SaveChanges();
            foreach (var item in sepet.Urunler)
            {
                SiparisDetay sd = new SiparisDetay();
                sd.UrunID = item.Urun.Id;
                sd.Adet = item.Adet;
                sd.Fiyat = item.Urun.SatiFiyat;
                sd.İndirim = item.Indirim;
                sd.SatisID = s.Id;
                Context.Baglanti.SiparisDetays.Add(sd);
                Context.Baglanti.SaveChanges();
            }
            HttpContext.Session.Remove("AktifSepet");

            Sati sat =Context.Baglanti.Satis.OrderByDescending(t => t.SatisTarihi).FirstOrDefault(x => x.MusteriID == m.Id);
            Session.Add("siparisId", sat.Id);

            

            return RedirectToAction("SiparisOnay");
        }

        public ActionResult SiparisOnay()
        {
            ViewBag.spt = Session["siparisId"];
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult OnayBekleyenSiparisler()
        {

            List<Sati> satis = Context.Baglanti.Satis.ToList();
            List<SiparisDetay> sdetay = Context.Baglanti.SiparisDetays.ToList();
            List<Urun> urunu = Context.Baglanti.Uruns.ToList();
            List<Musteri> ms = Context.Baglanti.Musteris.ToList();
            var satlist = from e in satis
                          join d in sdetay on e.Id equals d.SatisID into table1
                          from d in table1.ToList()
                          join i in urunu on d.UrunID equals i.Id into table2
                          from i in table2.ToList()
                          join g in ms on e.MusteriID equals g.Id into table3
                          from g in table3.ToList()
                          select new ViewModel
                          {
                              satis = e,
                              sdetay = d,
                              urunu = i,
                              ms = g
                          };
            ViewBag.Sdurum = Context.Baglanti.SiparisDurums.ToList();
            ViewBag.Odurum = Context.Baglanti.OdemeDurums.ToList();
            return View(satlist.OrderByDescending(x => x.satis.SatisTarihi));
        }

        [HttpPost]
        public ActionResult OnayBekleyenSiparisler(int odid, int sdid, int satid)
        {
            Sati sati = Context.Baglanti.Satis.FirstOrDefault(x => x.Id == satid);
            sati.OdemeDurumID = odid;
            sati.SiparisDurumID = sdid;
            Context.Baglanti.SaveChanges();
            return RedirectToAction("OnayBekleyenSiparisler");
        }

        public ActionResult KargoyaVerilenSiparisler()
        {
            List<Sati> satis = Context.Baglanti.Satis.ToList();
            List<SiparisDetay> sdetay = Context.Baglanti.SiparisDetays.ToList();
            List<Urun> urunu = Context.Baglanti.Uruns.ToList();
            List<Musteri> ms = Context.Baglanti.Musteris.ToList();
            var satlist = from e in satis
                          join d in sdetay on e.Id equals d.SatisID into table1
                          from d in table1.ToList()
                          join i in urunu on d.UrunID equals i.Id into table2
                          from i in table2.ToList()
                          join g in ms on e.MusteriID equals g.Id into table3
                          from g in table3.ToList()
                          select new ViewModel
                          {
                              satis = e,
                              sdetay = d,
                              urunu = i,
                              ms = g
                          };
            ViewBag.Kargo = Context.Baglanti.Kargoes.ToList();
            ViewBag.Odurum = Context.Baglanti.OdemeDurums.ToList();
            return View(satlist.OrderByDescending(x => x.satis.SatisTarihi));
        }


        [HttpPost]
        public ActionResult KargoyaVerilenSiparisler(int kgid, int satid, int? kgTkp)
        {
            Sati sati = Context.Baglanti.Satis.FirstOrDefault(x => x.Id == satid);
            sati.KargoID = kgid;
            sati.KargoTakipNo = kgTkp;
            sati.SiparisDurumID = 4;
            Context.Baglanti.SaveChanges();
            return RedirectToAction("KargoyaVerilenSiparisler");
        }

        public ActionResult TamamlananSiparisler()
        {
            List<Sati> satis = Context.Baglanti.Satis.ToList();
            List<SiparisDetay> sdetay = Context.Baglanti.SiparisDetays.ToList();
            List<Urun> urunu = Context.Baglanti.Uruns.ToList();
            List<Musteri> ms = Context.Baglanti.Musteris.ToList();
            var satlist = from e in satis
                          join d in sdetay on e.Id equals d.SatisID into table1
                          from d in table1.ToList()
                          join i in urunu on d.UrunID equals i.Id into table2
                          from i in table2.ToList()
                          join g in ms on e.MusteriID equals g.Id into table3
                          from g in table3.ToList()
                          select new ViewModel
                          {
                              satis = e,
                              sdetay = d,
                              urunu = i,
                              ms = g
                          };
            ViewBag.Kargo = Context.Baglanti.Kargoes.ToList();
            ViewBag.Odurum = Context.Baglanti.OdemeDurums.ToList();
            return View(satlist.OrderByDescending(x => x.satis.SatisTarihi));
        }

    }
}
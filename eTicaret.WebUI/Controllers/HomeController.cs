using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eTicaret.WebUI.App_Classes;
using eTicaret.WebUI.Models;


namespace eTicaret.WebUI.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Sepet()
        {
            if (HttpContext.Session["AktifSepet"] != null)
                return View((Sepet)HttpContext.Session["AktifSepet"]);
            else
                return View();
        }

        public PartialViewResult Slider()
        {
            var data = Context.Baglanti.Resims.Where(x => x.BuyukYol.Contains("Slider")).ToList();
            return PartialView(data);
        }

        public PartialViewResult Urunler()
        {
            var data = Context.Baglanti.Uruns.ToList();
            Random rnd = new Random();
            int r = rnd.Next(data.Count);
            ViewBag.urn = data[r];
            ViewBag.urn1 = data[r].Resims.FirstOrDefault(x => x.Varsayilan == false);
            return PartialView(data.Take(4));
        }
        public PartialViewResult YeniUrunler()
        {
            var data = Context.Baglanti.Uruns.ToList();
            return PartialView(data);
        }

        public PartialViewResult Servisler()
        {
            return PartialView();
        }

        public PartialViewResult Markalar()
        {
            var data = Context.Baglanti.Markas.ToList();

            return PartialView(data);
        }

        public void SepeteEkle(int id)
        {
            SepetItem si = new SepetItem();
            Urun u = Context.Baglanti.Uruns.FirstOrDefault(x => x.Id == id);

            si.Urun = u;
            si.Adet = 1;
            si.Indirim = 0;
            Sepet s = new Sepet();
            s.SepeteEkle(si);
            MiniSepetWidget();
        }

        public void SepetUrunAdetDusur(int id)
        {
            if (HttpContext.Session["AktifSepet"] != null)
            {
                Sepet s = (Sepet)HttpContext.Session["AktifSepet"];
                if (s.Urunler.FirstOrDefault(x => x.Urun.Id == id).Adet > 1)
                {
                    s.Urunler.FirstOrDefault(x => x.Urun.Id == id).Adet--;
                }

                else
                {
                    SepetItem si = s.Urunler.FirstOrDefault(x => x.Urun.Id == id);
                    s.Urunler.Remove(si);
                }
            }
        }

        public PartialViewResult MiniSepetWidget()
        {
            if (HttpContext.Session["AktifSepet"] != null)
                return PartialView((Sepet)HttpContext.Session["AktifSepet"]);
            else
                return PartialView();
        }

        public ActionResult UrunDetay(string id)
        {
            Urun u = Context.Baglanti.Uruns.FirstOrDefault(x => x.Adi == id);
            List<Yorum> y = Context.Baglanti.Yorums.Where(x => x.UrunID == u.Id).ToList();
            ViewBag.Yorumlar = y;
            List<UrunOzellik> uos = Context.Baglanti.UrunOzelliks.Where(x => x.UrunID == u.Id).ToList();
            Dictionary<string, List<OzellikDeger>> ozellik = new Dictionary<string, List<OzellikDeger>>();
            //List<OzellikTip> tips = new List<OzellikTip>();
            List<OzellikDeger> degers = new List<OzellikDeger>();

            foreach (UrunOzellik uo in uos)
            {
                OzellikTip ot = Context.Baglanti.OzellikTips.FirstOrDefault(x => x.Id == uo.OzellikTipID);

                bool varlik = false;
                foreach (var item in ozellik)
                {
                    if (item.Key != ot.Adi)
                        varlik = true;
                    else
                        varlik = false;
                }
                if (varlik)
                    degers = new List<OzellikDeger>();

                foreach (OzellikDeger deger in ot.OzellikDegers)
                {
                    OzellikDeger od = Context.Baglanti.OzellikDegers.FirstOrDefault(x => x.OzellikTipID == ot.Id && x.Id == uo.OzellikDegerID);
                    if (!degers.Any(x => x.Id == od.Id))//yoksa ekle, varsa ekleme
                        degers.Add(od);
                }
                if (ozellik.Any(x => x.Key == ot.Adi))
                    ozellik[ot.Adi] = degers;
                else
                    ozellik.Add(ot.Adi, degers);

                //degers = new List<OzellikDeger>();//listeyi boşalttık.

            }
            ViewBag.Ozellikler = ozellik;
            return View(u);
        }

        [HttpPost]
        public ActionResult YorumKaydet(int urid, string yadi, string yorum)
        {
            Musteri m = Context.Baglanti.Musteris.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
            Yorum y = new Yorum();
            y.Adi = yadi;
            y.MusteriID = m.Id;
            y.UrunID = urid;
            y.Yorum1 = yorum;
            Context.Baglanti.Yorums.Add(y);
            Context.Baglanti.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Kategoriler()
        {

            ViewBag.ktg = Context.Baglanti.Kategoris.ToList();
            ViewBag.mrk = Context.Baglanti.Markas.ToList();
            var data = Context.Baglanti.Uruns.ToList();
            Session.Remove("siralaKat");
            Session.Remove("siralaMar");
            Session.Remove("arama");
            return View(data);
        }

        public PartialViewResult KategoriWidget(int? id, int? markaid, byte? siraid)
        {
            if (id != null && markaid != null || id != null || markaid != null)
            {
                Session.Remove("siralaKat");
                Session.Remove("siralaMar");
                Session.Add("siralaKat", id);
                Session.Add("siralaMar", markaid);

            }
            object katnull = Session["siralaKat"];
            object marnull = Session["siralaMar"];
            if (katnull != null)
                id = (int)Session["siralaKat"];
            if (marnull != null)
                markaid = (int)Session["siralaMar"];


            List<Urun> urn = Context.Baglanti.Uruns.Where(x => x.KategoriID == id || markaid==null && id==null).ToList();
            if ((id > 0 | id != null) && markaid == null)
            {
                urn = Context.Baglanti.Uruns.Where(x => x.KategoriID == id).ToList();
            }
            else if ((id == 0 | id == null) && markaid != null)
            {
                urn = Context.Baglanti.Uruns.Where(x => x.MarkaID == markaid).ToList();
            }
            else if ((id > 0 | id != null) && markaid != null)
            {
                urn = Context.Baglanti.Uruns.Where(x => x.MarkaID == markaid && x.KategoriID == id).ToList();
            }

            if (siraid == 1)
            {
                urn = urn.OrderByDescending(x => x.SatiFiyat).ToList();

            }
            else if (siraid == 2)
            {
                urn = urn.OrderBy(x => x.SatiFiyat).ToList();

            }

            return PartialView(urn);

        }

        public ActionResult Arama(string searching, int? id)
        {

            ViewBag.ktg = Context.Baglanti.Kategoris.ToList();
            ViewBag.mrk = Context.Baglanti.Markas.ToList();
            if (searching != null)
            {
                Session.Remove("arama");
                Session.Add("arama", searching);

            }
            searching = Session["arama"].ToString();
            List<Urun> urn = Context.Baglanti.Uruns.Where(x => x.Adi.Contains(searching) || x.Kategori.Adi.Contains(searching) || searching == null).ToList();
            var strings = searching.Split(' ');
            foreach (var item in strings)
            {
                urn = urn.Concat(Context.Baglanti.Uruns.Where(x => x.Adi.Contains(item) || x.Kategori.Adi.Contains(item))).ToList();
            }

            urn = urn.Distinct().ToList();

            if (id == 1)
            {
                urn = urn.OrderByDescending(x => x.SatiFiyat).ToList();

            }
            else if (id == 2)
            {
                urn = urn.OrderBy(x => x.SatiFiyat).ToList();

            }

            return View(urn);
        }

        public ActionResult Iletisim()
        {
            return View();
        }

        public ActionResult YeniMenu()
        {
            return View();
        }
        //public PartialViewResult Orderby(int? ordering)
        //{
        //    List<Urun> urn = Context.Baglanti.Uruns.Where(x => x.Adi.Contains(searching) || searching == null).ToList();
        //    if (ordering == 1)
        //    {
        //        urn = Context.Baglanti.Uruns.OrderByDescending(x => x.SatiFiyat).ToList();
        //    }
        //    else if (ordering == 2)
        //    {
        //        urn = Context.Baglanti.Uruns.OrderBy(x => x.SatiFiyat).ToList();
        //    }
        //    return PartialView();
        //}
    }

}
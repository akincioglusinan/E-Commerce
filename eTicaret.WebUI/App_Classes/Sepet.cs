using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTicaret.WebUI.Models;

namespace eTicaret.WebUI.App_Classes
{
    public class Sepet
    {
        public static Sepet AktifSepet
        {
            get
            {
                HttpContext ctx = HttpContext.Current;//siteye giriş yapmış kullanıcı bilgilerini tutar
                if (ctx.Session["AktifSepet"] == null)
                    ctx.Session["AktifSepet"] = new Sepet();
                return (Sepet)ctx.Session["AktifSepet"];
            }
        }

        private List<SepetItem> urunler = new List<SepetItem>();

        public List<SepetItem> Urunler
        {
            get
            {
                return urunler;
            }
            set { urunler = value; }
        }
        public void SepeteEkle(SepetItem si)
        {
            if (HttpContext.Current.Session["AktifSepet"] != null)
            {
                Sepet s = (Sepet)HttpContext.Current.Session["AktifSepet"];
                if (s.Urunler.Any(x => x.Urun.Id == si.Urun.Id))
                    s.Urunler.FirstOrDefault(x => x.Urun.Id == si.Urun.Id).Adet++;//Aynı ürünü 2. defa eklerse artır.
                else
                {
                    s.Urunler.Add(si);
                }
            }
            else
            {
                Sepet s = new Sepet();
                s.Urunler.Add(si);

                HttpContext.Current.Session["AktifSepet"] = s;
            }


        }

        public decimal ToplamTutar
        {
            get
            {
                return Urunler.Sum(x => x.Tutar);
            }
        }
        public int SiparisID { get; set; }
        public int AdresID { get; set; }
    }

    public class SepetItem
    {
        public Urun Urun { get; set; }
        public int Adet { get; set; }
        public double Indirim { get; set; }
        public decimal Tutar
        {
            get
            {
                return Urun.SatiFiyat * Adet * (decimal)(1 - Indirim);
            }
        }

        

    }

}
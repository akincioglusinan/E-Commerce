using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTicaret.WebUI.Models;

namespace eTicaret.WebUI.App_Classes
{
    public class Kullanici
    {
        public string KullaniciAdi { get; set; }
        public string Parola { get; set; }
        public string Email { get; set; }
        public string GizliSoru { get; set; }
        public string GizliCevap { get; set; }
    }

    public class ViewModel
    {
        public Sati satis { get; set; }
        public SiparisDetay sdetay { get; set; }
        public Urun urunu { get; set; }
        public Musteri ms { get; set; }
        
    }
}
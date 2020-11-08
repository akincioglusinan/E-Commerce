using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTicaret.WebUI.Models;

namespace eTicaret.WebUI.App_Classes
{
    public class Context
    {
        private static eTicaretEntities baglanti;

        public static eTicaretEntities Baglanti
        {
            get
            {
                if (baglanti==null)
                {
                    baglanti = new eTicaretEntities();   
                }
                return baglanti;
            }
            set { baglanti = value; }
        }

    }

    public class Context1
    {
        private static eTicaretEntities1 baglanti;

        public static eTicaretEntities1 Baglanti
        {
            get
            {
                if (baglanti == null)
                {
                    baglanti = new eTicaretEntities1();
                }
                return baglanti;
            }
            set { baglanti = value; }
        }

    }
}
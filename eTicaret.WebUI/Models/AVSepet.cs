//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace eTicaret.WebUI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AVSepet
    {
        public int Id { get; set; }
        public Nullable<System.Guid> MusteriID { get; set; }
        public Nullable<int> UrunID { get; set; }
        public Nullable<int> Adet { get; set; }
    
        public virtual Musteri Musteri { get; set; }
        public virtual Urun Urun { get; set; }
    }
}

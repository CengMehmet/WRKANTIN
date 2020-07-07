using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WRKANTIN.Entities;

namespace WRKANTIN
{
    class KantinContext:DbContext
    {
        public KantinContext(string cs) : base(cs) { Database.SetInitializer(new DataInitializer()); }
        public DbSet<Kullanici> KullaniciSet { get; set; }
        public DbSet<Ogrenci> OgrenciSet { get; set; }
        public DbSet<Satilan> SatilanSet { get; set; }
        public DbSet<SatilanUrun> SatilanUrunSet { get; set; }
        public DbSet<Sepet> SepetSet { get; set; }
        public DbSet<Urun> UrunSet { get; set; }
        public DbSet<Ayarlar> AyarSet { get; set; }
        public DbSet<BakiyeHareket> BakiyeHareketSet { get; set; }

    }
}

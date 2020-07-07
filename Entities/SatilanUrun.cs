using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WRKANTIN.Entities
{
    class SatilanUrun
    {
        public int SatilanUrunId { get; set; }
        public int UrunId { get; set; }
        public virtual Urun urun { get; set; }
        public int SatilanId { get; set; }
        public Satilan Siparis { get; set; }
        public int Adet { get; set; }
    }
}

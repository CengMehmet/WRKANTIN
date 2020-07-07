using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WRKANTIN.Entities
{
    class Ogrenci
    {
        public int OgrenciId { get; set; }
        public string No { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string KartNo { get; set; }
        public string Sinif { get; set; }
        public string Tel { get; set; }
        public decimal Bakiye { get; set; }

    }
}

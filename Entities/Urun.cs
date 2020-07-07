using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WRKANTIN.Entities
{
    class Urun
    {
        [Key]
        public int UrunId { get; set; }
        public string Barkod { get; set; }
        public string Ad { get; set; }
        public string Kategori { get; set; }
        public bool Barkodlu   { get; set; }
        public decimal Fiyat { get; set; }
        public int Stok { get; set; }
        public override string ToString()
        {
            return Ad;
        }
    }
}

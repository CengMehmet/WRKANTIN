using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WRKANTIN.Entities
{
    class Satilan
    {
        [Key]
        public int SatilanId { get; set; }
        public decimal ToplamTutar { get; set; }
        public DateTime Tarih { get; set; }
        public int OgrenciId { get; set; }
        public virtual Ogrenci Alici { get; set; }
        public int SatilanUrunId { get; set; }
        public virtual List<SatilanUrun> satilanlar { get; set; }
    }
}

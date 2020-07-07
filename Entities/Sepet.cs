using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WRKANTIN.Entities
{
    class Sepet
    {
        [Key]
        public int SepetId { get; set; }
        public int UrunId { get; set; }
        public virtual Urun SepetUrun { get; set; }
        public int Adet { get; set; }

    }
}

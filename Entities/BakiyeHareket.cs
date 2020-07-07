using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WRKANTIN.Entities
{
    class BakiyeHareket
    {
        [Key]
        public int Id { get; set; }
        public Ogrenci ogrenci { get; set; }
        public decimal yuklenenBakiye { get; set; }
        public DateTime yuklemeTarihi { get; set; }
    }
}

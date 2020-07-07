using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WRKANTIN.Entities
{
    class Ayarlar
    {
        [Key]
        public int Id { get; set; }
        public string FisFirma { get; set; }
        public string FisBaslik { get; set; }
        public string YaziciAdi { get; set; }
        public string SonSatir { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPFYEND1.Models
{
    public class Sohbet
    {
        public string GonderenKullaniciAdi { get; set; }
        public string GonderenConnectionId { get; set; }
        public string AliciKullaniciAdi { get; set; }
        public string AliciConnectionId { get; set; }
        public string Mesaj { get; set; }
        public string MesajTarih { get; set; }
        public bool OkunmaDurum { get; set; }
    }
}
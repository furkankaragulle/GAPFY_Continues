using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPFYEND1.Models
{
    public class Kullanici
    {
        public string KullaniciAdi { get; set; }
        public string ConnectionId { get; set; }
        public Nullable<DateTime> GirisTarih { get; set; }
        public Nullable<DateTime> CikisTarih { get; set; }
        public OnOffDurum Durum { get; set; }
        public string Mesaj { get; set; }
        public string DurumIkon { get; set; }
    }
    public enum OnOffDurum
    {
        Online = 1,
        Offline = 2
    }
}
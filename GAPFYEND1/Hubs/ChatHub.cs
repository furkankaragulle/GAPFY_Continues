using GAPFYEND1.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPFYEND1.Hubs
{
    public class ChatHub : Hub
    {
        static List<Kullanici> kullanicis = new List<Kullanici>();
        static List<Sohbet> sohbets = new List<Sohbet>();

        public override System.Threading.Tasks.Task OnConnected()
        {
            Clients.Client(ConnectionIdGetir()).gonderenConnectionIdGoster(ConnectionIdGetir());
            return base.OnConnected();
        }
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            ListeKullaniciCikar(ConnectionIdGetir());
            return base.OnDisconnected(stopCalled);
        }
        public void ListeKullaniciEkle(string kullaniciAdi)
        {
            if (string.IsNullOrWhiteSpace(kullaniciAdi))
            {
                // Hata durumu veya kullanıcıya bildirim eklenebilir
                return;
            }

            // Aynı ConnectionId'ye sahip kullanıcıları kontrol et ve gerekiyorsa güncelle
            var existingUser = kullanicis.FirstOrDefault(u => u.ConnectionId == ConnectionIdGetir());
            if (existingUser != null)
            {
                existingUser.KullaniciAdi = kullaniciAdi;
                existingUser.GirisTarih = DateTime.Now;
                existingUser.Durum = OnOffDurum.Online;
                existingUser.Mesaj = " çevrimiçi";
                existingUser.DurumIkon = "online_icon";
            }
            else
            {
                // Yeni kullanıcıyı ekle
                kullanicis.Add(new Kullanici
                {
                    KullaniciAdi = kullaniciAdi,
                    ConnectionId = ConnectionIdGetir(),
                    GirisTarih = DateTime.Now,
                    Durum = OnOffDurum.Online,
                    Mesaj = " çevrimiçi",
                    DurumIkon = "online_icon"
                });
            }
            foreach (var item in kullanicis)
            {
                if (item.Durum == OnOffDurum.Offline)
                {

                    item.Mesaj = TimeSpan.Parse((DateTime.Now - Convert.ToDateTime(item.CikisTarih)).ToString()).Minutes + " dakika önce çevrimiçiydi";
                }
            }
            Clients.All.kisiListele(kullanicis);
        }

        public void ListeKullaniciCikar(string connectionId)
        {
            if (kullanicis.Exists(x => x.ConnectionId == connectionId))
            {
                Kullanici kullanici = kullanicis.Where(x => x.ConnectionId == connectionId).FirstOrDefault();
                kullanici.CikisTarih = DateTime.Now;
                kullanici.Durum = OnOffDurum.Offline;
                kullanici.DurumIkon = "online_icon offline";
                kullanici.Mesaj = " çevrimdışı";
            }
            foreach (var item in kullanicis)
            {
                if (item.Durum == OnOffDurum.Offline)
                {

                    item.Mesaj = TimeSpan.Parse((DateTime.Now - Convert.ToDateTime(item.CikisTarih)).ToString()).Minutes + " dakika önce çevrimiçiydi";
                }
            }
            Clients.All.kisiListele(kullanicis);
        }


        public void MesajGonder(string aliciKullaniciAdi, string aliciConnectionId, string mesaj)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            sohbets.Add(new Sohbet
            {
                AliciKullaniciAdi = aliciKullaniciAdi,
                AliciConnectionId = aliciConnectionId,
                Mesaj = mesaj,
                GonderenKullaniciAdi = kullanicis.Where(x => x.ConnectionId == ConnectionIdGetir()).Select(x => x.KullaniciAdi).Single().ToString(),
                GonderenConnectionId = ConnectionIdGetir(),
                MesajTarih = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")
            });

            List<string> konusmaMesaj = new List<string>();
            konusmaMesaj.Add(ConnectionIdGetir());
            konusmaMesaj.Add(aliciConnectionId);

            context.Clients.Clients(konusmaMesaj).tumKonusmaGoster(KonusmaListesiGetir(aliciConnectionId));
        }



        public void MesajListele(string aliciConnectionId)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            List<string> konusmaMesaj = new List<string>();
            konusmaMesaj.Add(ConnectionIdGetir());
            konusmaMesaj.Add(aliciConnectionId);
            context.Clients.Clients(konusmaMesaj).tumKonusmaGoster(KonusmaListesiGetir(aliciConnectionId));
        }
        public void MesajYaziyor(string aliciConnectionId, int adet)
        {

            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.Client(aliciConnectionId).mesajYaziyorGoster(adet);
        }
        private string ConnectionIdGetir()
        {
            return Context.ConnectionId;
        }

        public List<Sohbet> KonusmaListesiGetir(string aliciConnectionId)
        {

            List<Sohbet> liste1 = sohbets.Where(x => x.AliciConnectionId == aliciConnectionId && x.GonderenConnectionId == ConnectionIdGetir()).ToList();
            List<Sohbet> liste2 = sohbets.Where(x => x.AliciConnectionId == ConnectionIdGetir() && x.GonderenConnectionId == aliciConnectionId).ToList();

            return liste1.Concat(liste2).OrderBy(x => x.MesajTarih).ToList();
        }


    }
}
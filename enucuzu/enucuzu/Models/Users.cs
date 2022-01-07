using System;
using System.Collections.Generic;
using System.Text;


namespace enucuzu.Models
{
    public class Users
    {
        public string Key { get; set; }
        public string Kullanici_Adi { get; set; }
        public string Sifre { get; set; }
        public string Resim { get; set; }
        public Users()
        {
        }
        public Users(string _resim, string _ad, string _sifre)
        {
            Resim = _resim;
            Kullanici_Adi = _ad;
            Sifre = _sifre;
        }
    }
}
// Kullanıcı sınıfı tanımladık.kullanıcıya ait belirli özellikleri proppertylerle ekledik.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace enucuzu.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailPage : ContentPage
    {
        Database.DBFire db = new Database.DBFire();
        public Models.Products product { get; set; }
        public string Durum { get; set; }

        public DetailPage(Models.Products _product ,string _durum)
        {
            InitializeComponent();
            product = _product;
            image.Source = product.Product_img;
            barkod.Text = product.Barkod;
            P_Name.Text = product.Product_Name;
            Price.Text = product.Product_Price.ToString();
            Store_Name.Text = product.Product_Store;
            Kullanici.Text = product.Kullanici;
            Date.Text = product.Product_Date.ToString();
            Task<int> nesne = Task<int>.Factory.StartNew(() => db.kontrolfollow(product).Result);
            Durum = _durum;
            if (nesne.Result >0)
            {
                Durum = "Çıkar";
            }
            if (Durum == "Çıkar")
            {
                Buton.Text = "Takipten Çıkar";
                Buton.BackgroundColor = Color.OrangeRed;
            }
        }// detail sayfamızın veri ataması...
        private void Follow_Click(object sender, EventArgs e)
        {
            if (Durum == "Ekle")
            {
                db.add_Follow(product);
                DisplayAlert("", "Takip listednize eklendi", "Tamam");
                //Buton.IsEnabled = false;
                Buton.BackgroundColor = Color.OrangeRed;
                Buton.Text = "Takipten Çıkar";
                Durum = "Çıkar";
            }// takip listesine ekleme
            else if (Durum == "Çıkar")
            {
                db.del_Follow(product);
                DisplayAlert("", "Takip listednizden çıkartıldı", "Tamam");
                Buton.BackgroundColor = Color.DarkBlue;
                Buton.Text = "Takip Et";
                Durum = "Ekle";
            }// Takip listesinden çıkarma
        }
    }
}
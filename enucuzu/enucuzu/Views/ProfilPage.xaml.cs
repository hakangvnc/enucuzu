using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace enucuzu.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilPage : ContentPage
    {
        Database.DBFire db = new Database.DBFire();
        ObservableCollection<Models.Products> Liste =new ObservableCollection<Models.Products>();
        public ProfilPage()
        {
            InitializeComponent();
            k_Ad.Text = App.log_k_adi;
            k_photo.Source = App.log_k_resim;
            //var takip = db.takip_sayac();
            //Takip.Text = takip.ToString();
            Liste = db.getpaylasim();
            if (Liste != null)
            {
                p_list.ItemsSource = Liste;
            }
            Paylasim.Text = Liste.Count().ToString().Trim();
        }

        private async void Log_Out(object sender, EventArgs e)
        {
            App.Key = null;
            App.log_k_adi = null;
            App.log_k_resim = null;
            App.log_k_sifre = null;
            await Navigation.PushModalAsync(new LoginPage());
            // çıkış yapıldıdıgında login yönlendirilirrrrr
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AccountPage());
        }

        private async void p_list_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var deger = await DisplayAlert("Uyarı","Paylaşımı silmek isteddiğinize emin misiniz ?","Evet","Hayır");
            if (deger)
            {
                db.del_share(Liste[e.ItemIndex]);
                db.del_product(Liste[e.ItemIndex]);
            }
        }
    }// Hesap işlemleri için yönlendirme
}
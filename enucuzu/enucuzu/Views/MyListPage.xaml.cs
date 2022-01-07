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
    public partial class MyListPage : ContentPage
    {
        Database.DBFire db = new Database.DBFire();
        ObservableCollection<Models.Products> Liste;
        List<Models.Products> Liste_Son;
        public MyListPage()
        {
            InitializeComponent();
            Liste = db.gettakip();
            if (Liste != null)
            {
                f_list.ItemsSource = Liste;
            }
        }
        //  Takip edlenlen ürünleri item source'a atıyoruz...
        private void Detail_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("", "", "Ok");
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length >= 3)
            {
                f_list.ItemsSource = Liste.Where(item => item.Product_Name.Contains(e.NewTextValue) || item.Barkod.Contains(e.NewTextValue));
            }
            else if (e.NewTextValue.Length == 2)
            {
                f_list.ItemsSource = Liste;
            }
        }
        // takip ettğimiz ürünlerin listesini çekip itemsource'ne atıyoruz...

        private async void ImageButton1_Clicked(object sender, EventArgs e)
        {
            var deger = await DisplayActionSheet("Filtreleme", "", "", "Ucuzdan pahalıya", "Pahalıdan ucuza", "En yakın tarihe göre", "En Uzak tarihe göre", "İsme göre sırala");
            if (deger == "Ucuzdan pahalıya")
            {
                Liste_Son = Liste.OrderBy(item => item.Product_Price).ToList();
                f_list.ItemsSource = Liste_Son;
            }
            if (deger == "Pahalıdan ucuza")
            {
                Liste_Son = Liste.OrderByDescending(item => item.Product_Price).ToList();
                f_list.ItemsSource = Liste_Son;
            }
            if (deger == "En yakın tarihe göre")
            {
                Liste_Son = Liste.OrderByDescending(item => item.Product_Date).ToList();
                f_list.ItemsSource = Liste_Son;
            }
            if (deger == "En Uzak tarihe göre")
            {
                Liste_Son = Liste.OrderBy(item => item.Product_Date).ToList();
                f_list.ItemsSource = Liste_Son;
            }
            if (deger == "İsme göre sırala")
            {
                Liste_Son = Liste.OrderBy(item => item.Product_Name).ToList();
                f_list.ItemsSource = Liste_Son;
            }
        }// Filtreleme işlemleri 
        private async void f_list_ItemTapped(object sender, ItemTappedEventArgs e)
        {

            //DisplayAlert("",Liste[e.ItemIndex].Product_Name,"ok");
            if (f_list.ItemsSource == Liste)
            {
                Liste_Son = Liste.ToList();
            }
            await Navigation.PushModalAsync(new DetailPage(Liste_Son[e.ItemIndex],"Çıkar"));
        }
        private void Barkod_Clicked(object sender, EventArgs e)
        {
            if (Scanframe.IsVisible == true)
            {
                Scanframe.IsVisible = false;
                Scanner.IsEnabled = false;
            }
            else
            {
                Scanframe.IsVisible = true;
                Scanner.IsEnabled = true;
                GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
            }
        }
        // listeden seçilen item için 
        private async void CameraView_OnDetected(object sender, GoogleVisionBarCodeScanner.OnDetectedEventArg e)
        {
            // liste döndüdüğü içi n 0 yadık ve ilk elmanı aldık.
            GoogleVisionBarCodeScanner.BarcodeResult _barkod = e.BarcodeResults[0];
            await DisplayAlert("Barkod Okundu", _barkod.DisplayValue, "Tamam");
            find.Text = _barkod.DisplayValue;
            Scanframe.IsVisible = false;
            Scanner.IsEnabled = false;
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);
            e.BarcodeResults.Clear();
        }
    }
}
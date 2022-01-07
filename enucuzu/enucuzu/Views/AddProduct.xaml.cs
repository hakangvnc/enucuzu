using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace enucuzu.Views
{
    public partial class AddProduct : ContentPage
    {
        public Stream File { get; set; }
        // Çekilen veya seçilen fotografı tumak için olan Straem öğemiz 
        public AddProduct()
        {
            InitializeComponent();
        }
        private async void image_Clicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("", "Vazgeç", "", "Fotoğraf Çek", "Galeriden Seç");
            Console.WriteLine("Action: " + action);
            if (action == "Fotoğraf Çek")
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Resimler",
                    SaveToAlbum = true,
                    CompressionQuality = 75,
                    CustomPhotoSize = 50,
                    PhotoSize = PhotoSize.MaxWidthHeight,
                    MaxWidthHeight = 2000,
                    DefaultCamera = CameraDevice.Front
                });

                if (file == null)
                    return;

                // DisplayAlert("File Location", file.Path, "OK"); 

                image.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    File = file.GetStream();
                    file.Dispose();
                    return stream;
                });

            }// Fotoğraf çekme işlemleri
            if (action == "Galeriden Seç")
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("ERROR", "Pick Photo is NOT supported", "OK");
                    return;
                }

                var file = await CrossMedia.Current.PickPhotoAsync();
                if (file == null)
                {
                    return;
                }
                image.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    File = file.GetStream();
                    file.Dispose();
                    return stream;
                });
            }
        }// Galeriden fotoğraf seçme işlemleri
        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (barkod.Text == null || P_Name.Text == null || Price.Text == null || Store_Name.Text == null || File == null)
            {
                await DisplayAlert("", "Lütfen boş alan bırakmayınız", "Tamam");
            }
            else
            {
                Database.DBFire db = new Database.DBFire();
                Task<int> _Id = Task<int>.Factory.StartNew(() => db.kontrol_Id().Result);
                var Id = 1;
                if (_Id != null) { Id = _Id.Result; }
                Task<int> task = Task<int>.Factory.StartNew(() => db.kontrolPrice(barkod.Text, Convert.ToDouble(Price.Text), Store_Name.Text).Result);
                if (task == null)
                {
                    await DisplayAlert("Uyarı", "Daha önce aynı markette daha düşük fiyat girilmiş.", "Tamam");
                    barkod.Text = "";
                    P_Name.Text = "";
                    Price.Text = "";
                    Store_Name.Text = "";
                }
                else
                {
                    var task1 = await db.up_Product_Image(File, barkod.Text);
                    if (task1 == null)
                    {
                        await DisplayAlert("", "Fotograf yüklenemedi tekrar deneyiniz", "Tamam");
                    }
                    Models.Products product = new Models.Products((Id+1), task1, barkod.Text, P_Name.Text, Convert.ToDouble(Price.Text), Store_Name.Text ,App.log_k_adi);
                    db.setProducts(product);
                    await DisplayAlert("", "Kayit başarılı ", "Tamam");
                    image.Source = "usericon.png";
                    barkod.Text = null;
                    P_Name.Text = null;
                    Price.Text = null;
                    Store_Name.Text = null;
                }
            }
        }
        //ürünümüzü veritabanına ekleme işlemleri ................................(yukarı)
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
        private async void CameraView_OnDetected(object sender, GoogleVisionBarCodeScanner.OnDetectedEventArg e)
        {
            // liste döndüdüğü içi n 0 yadık ve ilk elmanı aldık.
            GoogleVisionBarCodeScanner.BarcodeResult _barkod = e.BarcodeResults[0];
            await DisplayAlert("Barkod Okundu", _barkod.DisplayValue, "Tamam");
            barkod.Text = _barkod.DisplayValue;
            Scanframe.IsVisible = false;
            Scanner.IsEnabled = false;
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);
            e.BarcodeResults.Clear();
        }
    }//Barkod okutma işlemleri 
}
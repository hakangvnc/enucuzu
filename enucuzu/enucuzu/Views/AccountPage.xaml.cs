using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace enucuzu.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountPage : ContentPage
    {
        public Stream File { get; set; }
        Database.DBFire db = new Database.DBFire();
        public AccountPage()
        {
            InitializeComponent();
        }

        private void foto_gör_Clicked(object sender, EventArgs e)
        {
            if (foto_islem.IsVisible == true)
            {
                foto_islem.IsVisible = false;
            }
            else
            {
                foto_islem.IsVisible = true;
                sifre_islem.IsVisible = false;
                sil_islem.IsVisible = false;
                image.Source = App.log_k_resim;
            }
        }
        private void sifre_gör_Clicked(object sender, EventArgs e)
        {
            if (sifre_islem.IsVisible == true)
            {
                sifre_islem.IsVisible = false;
            }
            else
            {
                sil_islem.IsVisible = false;
                foto_islem.IsVisible = false;
                sifre_islem.IsVisible = true;
            }
        }
        private void sil_gör_Clicked(object sender, EventArgs e)
        {
            if (sil_islem.IsVisible == true)
            {
                sil_islem.IsVisible = false;
            }
            else
            {
                sil_islem.IsVisible = true;
                foto_islem.IsVisible = false;
                sifre_islem.IsVisible = false;
            }
        }
        // Stack loyoutlardan biri açıldıgında diğerleri kapanması için yukardaki kodlar yazılmıştır.

        private async void imagesec_Clicked(object sender, EventArgs e)
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
                    SaveToAlbum = false,
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
                if (File != null)
                {
                    foto_but.IsEnabled = true;
                }
            }
            else if (action == "Galeriden Seç")
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
                if (File != null)
                {
                    foto_but.IsEnabled = true;
                }
            }
        }

        private void Sifre_but_Clicked(object sender, EventArgs e)
        {
            if (pass.Text == App.log_k_sifre)
            {
                if (newpass.Text.Length >= 8)
                {
                    if (repass.Text == newpass.Text)
                    {
                        var user = new Models.Users(App.log_k_resim, App.log_k_adi, newpass.Text);
                        App.log_k_sifre = newpass.Text;
                        db.updateUsers(user);
                        DisplayAlert("İşlem Başarılı", "Şifre güncelleme işlemi başarıyla gerçekleşti", "Tamam");
                    }
                    else
                    {
                        DisplayAlert("Uyarı", "Girmiş olduğunuz şifreler birbiriyle uyuşmuyor", "Tamam");
                    }
                }
                else
                {
                    DisplayAlert("Uyarı", "Yeni şifreniz en az 8 haneli olabilir", "Tamam");
                }
            }
            else
            {
                DisplayAlert("Uyarı", "Şuanda kullanmakta olduğunuz şifrenizi yanlış girdiniz.", "Tamam");
            }
        }

        // Şifre GÜncelleme işlemi 

        private async void foto_but_Clicked(object sender, EventArgs e)
        {
            var task = await db.uploadImage(File, App.log_k_adi);
            if (task == null)
            {
                await DisplayAlert("", "Fotograf yüklenemedi tekrar deneyiniz", "Tamam");
            }
            else
            {
                App.log_k_resim = task;
                var user = new Models.Users(task, App.log_k_adi, App.log_k_sifre);
                db.updateUsers(user);
                await DisplayAlert("İşlem Başarılı", "Fotoğraf güncelleme işlemi başarıyla gerçekleşti", "Tamam");
            }
        }

        private void pass_TextChanged(object sender, TextChangedEventArgs e)
        {
            Sifre_but.IsEnabled = true;
        }

        private void check1_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            sil_but.IsEnabled = true;
        }
        private async void sil_but_Clicked(object sender, EventArgs e)
        {
            if (check1.IsChecked == true && check2.IsChecked == true)
            {
                // Paylaşim silme işlemleri gelecek
            }
            else if (check1.IsChecked == true)
            {
                db.delUsers(App.log_k_adi);
                db.delimg(App.log_k_adi);
                await DisplayAlert("İşlem Başarılı","Hesabınız silindi", "Tamam");
                App.Key = null;
                App.log_k_adi = null;
                App.log_k_resim = null;
                App.log_k_sifre = null;
                await Navigation.PushModalAsync(new AboutPage());
            }
            // Paylaşımlar hariç hesap silme işlemleri..
            // login olan kullanıcı bilgilerini sıfırladık
        }
    }
}
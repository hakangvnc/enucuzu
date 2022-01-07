using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace enucuzu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public Stream File { get; set; }
        // çekilene veya seçilen fotografı tutmak için bir Stream öğesi

        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void ImageButton_Clicked(object sender, EventArgs args)
        {
            string action = await DisplayActionSheet("", "Vazgeç", "", "Fotoğraf Çek", "Galeriden Seç");
            Console.WriteLine("Action: " + action);
            if (action== "Fotoğraf Çek")
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
                
            }
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
        }
        async void Button_Clicked(object sender, EventArgs e)
        {
            if (username.Text == "" || pass.Text == "" || repass.Text== "") {
                await DisplayAlert("", "Lütfen boş alan bırakmayınız", "Tamam");
            }
            else if (pass.Text.Length >= 8)
            {
                if (repass.Text == pass.Text)
                {
                    Database.DBFire db = new Database.DBFire();
                    Task<int> task1 = Task<int>.Factory.StartNew(() => db.kontrolusername(username.Text).Result);
                    int deger = task1.Result;
                    if (task1.Result > 0)
                    {
                        await DisplayAlert("", "Kullanıcı adı alınmış.", "Tamam");
                        username.Text = "";
                    }
                    else {
                        var task = await db.uploadImage(File,username.Text);
                        if (task == null)
                        {
                            await DisplayAlert("","Fotograf yüklenemedi tekrar deneyiniz","Tamam");
                        }
                        Models.Users user = new Models.Users(task, username.Text, pass.Text);
                        db.setUsers(user);
                        await DisplayAlert("", "Kayit başarılı giriş sayfasına yönlendiriliyorsunuz", "Tamam");
                        await Navigation.PushModalAsync(new LoginPage());
                        username.Text = task;
                        pass.Text = "";
                        repass.Text = "";
                    }
                }
            else
            {
                await DisplayAlert("", "Şifreler Uyuşmuyor Tekrar deneyiniz.", "Tamam");
            }
            }
            else
            {
                await DisplayAlert("", "Şifre 8 hane ve fazlası olmalıdır.", "Tamam");
            }
        }
    }//KUllanıcıyı kaydetme işlemleri
}
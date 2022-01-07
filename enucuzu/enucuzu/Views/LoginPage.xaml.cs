using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace enucuzu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        async void Login_Click(object sender , EventArgs e)
        {
            Database.DBFire db = new Database.DBFire();

            Task<Models.Users> user = Task<Models.Users>.Factory.StartNew(() => db.loginUser(username.Text,pass.Text).Result);
            var user_log = user.Result;
            if (user_log != null)
            {
                App.Key = user_log.Key;
                App.log_k_adi = user_log.Kullanici_Adi;
                App.log_k_resim = user_log.Resim;
                App.log_k_sifre = user_log.Sifre;
                await Navigation.PushModalAsync(new MainPage());
            }
            else
            {
                Console.WriteLine("KULLANICI ADI VEYA ŞİFRE YANLIŞ");
                await DisplayAlert("Uyarı", "Kullanıcı Adı veya Sifre Yanlış.", "Tekrar Dene");
                username.Text = "";
                pass.Text = "";
            }
        }
    }
}
// Giriş Yapmaya Çalışan kullanıcının bilgilerini veritabanından kontrol ediyoruz doğruysa giriş izni veriyoruz. Session olarak App.cs dosyamızda oluşturdugumuz propertyler ile
// giriş tyapan kullanıcılının bilgilerini tutuyoyoruz.uygulama içerisinde kullanmka için.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Database.Query;
using Xamarin.Forms;
using Firebase.Storage;
using Plugin.Media.Abstractions;
using System.IO;

namespace enucuzu.Database  
{
    public class DBFire
    {
        FirebaseClient Client;
        public DBFire()
        {
            Client = new FirebaseClient("https://enucuzu-v01-default-rtdb.firebaseio.com/");
            // Firebase için bağlantı url mizi atııyoruz
        }
        public void setUsers(Models.Users Kisi)
        {
            Client.Child("Users").PostAsync(Kisi);
            // Firebase de Users adında bir klasör oluşurup içine Users sınıfından tanımlan mış nesnemizi kaydediyoruz.
            // Not : Fire base nesne tabanlı bir data base oldugu için class lardan tanımlanan nesneleri propertyleri ile direk olarak tutabiliyor.
        }

        public async Task<int> kontrolusername(string _name)
        {
            var Varmi = (await Client.Child("Users").OnceAsync<Models.Users>()).Where(x => x.Object.Kullanici_Adi == _name).Count();
            return Varmi;
        }
        // Veritabanı işlemlerimiz için firebase database kullandık fire base bağlandık. yeni kayıt oluştururken kullanıcı adının unique bir değer olması için 
        // daha önce bu isimin kayıtlı olup olmadıgını kontrol ediyoruz yok ise kullanımına izin veriyozruz var ise kulaınıcı adı zaten alınmış hatası verdiriyoruz.

        public async Task<Models.Users> loginUser(string _name, string _sifre)
        {
            var user = (await Client.Child("Users").OnceAsync<Models.Users>()).Where(x => x.Object.Kullanici_Adi == _name && x.Object.Sifre == _sifre).Select(item =>
                    new Models.Users
                    {
                        Key = item.Key,
                        Kullanici_Adi = item.Object.Kullanici_Adi,
                        Resim = item.Object.Resim,
                        Sifre = item.Object.Sifre
                    }
            ).FirstOrDefault();
            return user;
        }// Kullanıcı adı ve Şİfre dogru ise anasayfamıza yönlendirme yapıyoruz.Eğer yanlışsa hata verdiryoruz.

        // update yapma
        public async void updateUsers(Models.Users Kisi)
        {
            var itemtoupdate = (await Client.Child("Users").OnceAsync<Models.Users>()).Where(x => x.Object.Kullanici_Adi == Kisi.Kullanici_Adi).FirstOrDefault();
            if (itemtoupdate != null)
            {
                await Client.Child("Users").Child(itemtoupdate.Key).PutAsync(Kisi);
            }
        }
        // KUllanıcı her  hangi bir bilgisini güncellemek istediğinde Users sınıfında güncel bilgileri ile yeni bir nesne tanımlıyoruz ve kullanıcı ad benzersiz
        // alan oldugu için, veritabanında kullanıcı adı aynı olan user nesnesi ile güncel user nesne misizi değiştiriyoruz.
        public async void delUsers(string _ad)
        {
            var deltoitem = (await Client.Child("Users").OnceAsync<Models.Users>()).Where(x => x.Object.Kullanici_Adi == _ad).FirstOrDefault();
            if (deltoitem != null)
            {
                await Client.Child("Users").Child(deltoitem.Key).DeleteAsync();
            }
        }
        //Kulllannıcı adı ile kullanıcımızı veri tabannda siliyoruz ve fotorafımızıda veri tabanından siliyoruz.
        public async Task<string> uploadImage(Stream _img, string _username)
        {
            var st_img = await new FirebaseStorage("enucuzu-v01.appspot.com")
                .Child("Users")
                .Child(_username + ".jpg")
                .PutAsync(_img);
            return st_img;
        }
        //fotograflarımızı firebase storage yüklediğimiz yer burda şöle bi güzellik var aynı isimde başka dosya varsa güncelliyo yoksa yeni dosyayı ekliyor
        // yine unique oldugu için 
        public async void delimg(string _ad)
        {
            await new FirebaseStorage("enucuzu-v01.appspot.com")
                .Child("Users")
                .Child(_ad+".jpg")
                .DeleteAsync();
         // foto silme işlemmiz ürün silme ve kullanıcı silme işlelrinde otomatik çalışırız.
        }
        //***************************************************************************ürün ekleme******************************************************
        // veri ekleme
        public void setProducts(Models.Products product)
        {
            Client.Child("Products").PostAsync(product);
            setshares(product);
        }
        //ürünümüzü veritabanına ekliyoruz.
        public void setshares(Models.Products product)
        {
            Client.Child("Users").Child(App.Key).Child("Share").PostAsync(product);
        }
        // kişiye özel paylaşım listesine ekleme
        public async Task<int> kontrol_Id()
        {
            var Id = (await Client.Child("Products").OnceAsync<Models.Products>()).Where(x => x.Object.Product_Id != 0).Count();
            return Id;
        }
        //veri tabanında oln nesnellerin sayını alıp 1 artıtırıyotuz ve ürünümüze id olarak atıyoruz.
        //eski verileri kontrl etme

        public async Task<int> kontrolPrice(string barkod, double price, string store)
        {
            var Varmi = (await Client.Child("Products").OnceAsync<Models.Products>()).Where(x => x.Object.Barkod == barkod && x.Object.Product_Price <= price && x.Object.Product_Store.ToLower() == store.ToLower()).Count();
            return Varmi;
        }
        // Ü rüneklerken daha düşü bir fitatın oup olmadıgıa bakıyoruz.


        public async Task<string> up_Product_Image(Stream _img, string file_name)
        {
            var st_img = await new FirebaseStorage("enucuzu-v01.appspot.com")
                .Child("Products")
                .Child(file_name + ".jpg")
                .PutAsync(_img);
            return st_img;
        }


        // ürünümüzün fotosunu veri tabanına yüklüyoruz.
        public ObservableCollection<Models.Products> getAnlik()
        {
            var data = Client
                .Child("Products")
                .AsObservable<Models.Products>()
                .AsObservableCollection();
            return data;
        }

        public async Task<int> kontrolfollow(Models.Products _product)
        {
            var Varmi = (await Client.Child("Users").Child(App.Key).Child("Follow").OnceAsync<Models.Products>()).Where(x => x.Object.Barkod == _product.Barkod).Count();
            return Varmi;
        }

        // Takip olayları takip listesine ekleme ve çıkarma ;
        public void add_Follow(Models.Products _follow)
        {
            var data = Client
                .Child("Users")
                .Child(App.Key)
                .Child("Follow")
                .PostAsync(_follow);
        }
        // takip listesine kullanıcıları ekleme
        public async void del_Follow(Models.Products _follow)
        {
            var delitem = (await Client.Child("Users").Child(App.Key).Child("Follow").OnceAsync<Models.Products>()).Where(x => x.Object.Barkod == _follow.Barkod).FirstOrDefault();
            var data = Client
                .Child("Users")
                .Child(App.Key)
                .Child("Follow")
                .Child(delitem.Key)
                .DeleteAsync();
        }
        public async void del_share(Models.Products _share)
        {
            var delitem = (await Client.Child("Users").Child(App.Key).Child("Share").OnceAsync<Models.Products>()).Where(x => x.Object.Barkod == _share.Barkod).FirstOrDefault();
            var data =Client
                .Child("Users")
                .Child(App.Key)
                .Child("Share")
                .Child(delitem.Key)
                .DeleteAsync();
        }
        public async void del_product(Models.Products _share)
        {
            var delitem = (await Client.Child("Products").OnceAsync<Models.Products>()).Where(x => x.Object.Barkod == _share.Barkod).FirstOrDefault();
            var data2 = Client
                .Child("Products")
                .Child(delitem.Key)
                .DeleteAsync();
            del_img_product(_share.Barkod.ToString());
        }
        public async void del_img_product(string _ad)
        {
            await new FirebaseStorage("enucuzu-v01.appspot.com")
                .Child("Products")
                .Child(_ad + ".jpg")
                .DeleteAsync();
            // foto silme işlemmiz ürün silme ve kullanıcı silme işlelrinde otomatik çalışırız.
        }
        // takip listesinden takip edilen ürünleri silme
        public int paylasim_sayac()
        {
            var Varmi = Client.Child("Products").OnceAsync<Models.Products>();
            return Varmi.Result.Count();
        }
        public int takip_sayac()
        {
            var Varmi = Client.Child("Users").Child(App.Key).Child("Follow").AsObservable<Models.Products>().AsObservableCollection();
            return Varmi.Count<Models.Products>();
        }
    // takip edilen ürünlerin sayısını alma 
        public ObservableCollection<Models.Products> gettakip()
        {
            var data = Client
                .Child("Users")
                .Child(App.Key)
                .Child("Follow")
                .AsObservable<Models.Products>()
                .AsObservableCollection();
            return data;
        }// listem sayfasında anlık olarak takip edilen ürünleri çekiyoruz.
        public ObservableCollection<Models.Products> getpaylasim()
        {
            var data = Client
                .Child("Users")
                .Child(App.Key)
                .Child("Share")
                .AsObservable<Models.Products>()
                .AsObservableCollection();

            return data;
        }
    }
}
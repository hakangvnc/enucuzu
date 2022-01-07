using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace enucuzu
{
    public partial class App : Application
    {
        public static string Key { get; set; }
        public static string log_k_adi { get; set; }
        public static string log_k_sifre { get; set; }
        public static string log_k_resim { get; set; }
        //Session iiçin oluşturdugumuz propertylerimiz
        public App()
        {
            InitializeComponent();
            MainPage = new AboutPage();
            //ilk açılışta açılacak sayfa
        }
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

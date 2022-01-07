using System;
using System.Collections.Generic;
using System.Text;

namespace enucuzu.Models
{
    public class Products
    {
        public int Product_Id { get; set; }
        public string Product_img { get; set; }
        public string Barkod { get; set; }
        public string Product_Name { get; set; }
        public double Product_Price { get; set; }
        public string Product_Store { get; set; }
        public string Kullanici { get; set; }
        public DateTime Product_Date{ get; set; }
        public Products()
        {
            //boş nesne tanımlamak için 
        }
        public Products( int _Id, string _img,string _barkod , string _name , double _price, string _store ,string _kul)
        {
            Product_Id = _Id;
            Product_img = _img;
            Barkod = _barkod;
            Product_Name = _name;
            Product_Price = _price;
            Product_Store = _store;
            Kullanici = _kul;
            Product_Date = DateTime.Now;
        }
    }
}

// Ürünlerimize ait class yapımızı oluşturduk ve özlliklerilerimizi propertyler ile clasımıza ekledik. Constructer Method overload ederek gelen
// verilerilerimizi propertylere atadık.

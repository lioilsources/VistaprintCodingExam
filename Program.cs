using System;
using System.Collections.Generic;
using System.Linq;

namespace CodingExam
{
    static class CollectionEx
    {
        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
    }

    class Cart 
    {
        IList<IProduct> products = new List<IProduct>();
        IList<ISale> sales = new List<ISale>();

        //int index = 0;

        public decimal Calculate()
        {
            sales.Each(s => s.AttachToProduct(products.ToArray()));
            return products.Sum(p => p.GetPrice());
        }

        public void AddProduct(IProduct product)
        {
            products.Add(product);
        }

        public void AddSale(ISale sale)
        {
            sales.Add(sale);
        }

    }

    interface IProduct 
    {
        decimal GetPrice();
        void AddSale(ISale sale);
    }

    interface ISale 
    {
        void AttachToProduct(IProduct[] products);
        decimal GetSale(decimal price);
    }

    class Product : IProduct
    {
        decimal price;
        IList<ISale> sales = new List<ISale>();

        public Product(decimal price)
        {
            this.price = price;
        }

        public void AddSale(ISale sale)
        {
            sales.Add(sale);
        }

        public decimal GetPrice()
        {
            return sales.Aggregate(price, (price, sale) => sale.GetSale(price));
        }
    }

    class Sale1 : ISale
    {
        decimal percentSale;

        public Sale1(decimal sale)
        {
            this.percentSale = sale;
        }

        public void AttachToProduct(IProduct[] products)
        {
            foreach (var product in products)
            {
                product.AddSale(this);
            }
        }

        public decimal GetSale(decimal price)
        {
            return price * percentSale;
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var cart = new Cart();
            var sale1 = new Sale1(0.9m);
            var tshirt = new Product(10);

            cart.AddProduct(tshirt);
            cart.AddProduct(new Product(10));
            cart.AddSale(sale1);

            Console.WriteLine($"End of World! ${cart.Calculate()}");
        }
    }
}

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

        int index = 0;

        public decimal Calculate()
        {
            sales.Each(s => s.AttachToProduct(products.ToArray()));
            return products.Sum(p => p.GetPrice());
        }

        public void AddProduct(IProduct product)
        {
            products.Add(product);
            product.SetOrder(index++);
        }

        public void AddSale(ISale sale)
        {
            sales.Add(sale);
            sale.SetOrder(index++);
        }

    }

    interface ICartItem
    {
        void SetOrder(int order);
        int GetOrder();
    }

    interface IProduct : ICartItem
    {
        decimal GetPrice();
        void AddSale(ISale sale);
    }

    interface ISale : ICartItem
    {
        void AttachToProduct(IProduct[] products);
        decimal GetSale(decimal price);
    }

    class Product : CartItem, IProduct
    {
        readonly decimal price;
        readonly IList<ISale> sales = new List<ISale>();

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

    class CartItem : ICartItem
    {
        int order;

        public void SetOrder(int order)
        {
            this.order = order;
        }

        public int GetOrder()
        {
            return order;
        }
    }

    class Sale1 : CartItem, ISale
    {
        protected decimal percentSale;

        public Sale1(decimal sale)
        {
            this.percentSale = sale;
        }

        public virtual void AttachToProduct(IProduct[] products)
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

    class Sale2 : Sale1
    {
        public Sale2(decimal sale) : base(sale) { }

        public override void AttachToProduct(IProduct[] products)
        {
            var product = products.Where(p => p.GetOrder() > GetOrder()).OrderBy(o => o).FirstOrDefault();
            if (product != null)
                product.AddSale(this);
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var cart = new Cart();
            //var sale1 = new Sale1(0.9m);
            var sale2 = new Sale2(0.5m);
            var tshirt = new Product(10);

            cart.AddProduct(tshirt);
            cart.AddSale(sale2);
            cart.AddProduct(new Product(10));
            //cart.AddSale(sale1);

            Console.WriteLine($"End of World! ${cart.Calculate()}");
        }
    }
}

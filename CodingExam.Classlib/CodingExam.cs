using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Task: Calculate cart price. 
 * Details: Cart consists of products and sales. Product has price and sale discount some product/s in cart.
 * 
 * Sale1: Discount X% on every product in cart.
 * Sale2: Discount Y% only product on next position in cart.
 * Sale3: Discount only Zth product of specified type with -$S discount.
 */

namespace CodingExam.Classlib
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

    public class Cart
    {
        readonly IList<IProduct> products = new List<IProduct>();
        readonly IList<ISale> sales = new List<ISale>();

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

    public interface ICartItem
    {
        void SetOrder(int order);
        int GetOrder();
    }

    public interface IProduct : ICartItem
    {
        decimal GetPrice();
        void AddSale(ISale sale);
    }

    public interface ISale : ICartItem
    {
        void AttachToProduct(IProduct[] products);
        decimal GetSale(decimal price);
    }

    public class CartItem : ICartItem
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

    public class Product : CartItem, IProduct
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

    public class TShirt : Product
    {
        public TShirt(decimal price) : base(price) { }
    }

    public abstract class Sale : CartItem, ISale
    {
        public abstract void AttachToProduct(IProduct[] products);
        public abstract decimal GetSale(decimal price);
    }

    public class Sale1 : Sale
    {
        protected decimal percentSale;

        public Sale1(decimal sale)
        {
            this.percentSale = sale;
        }

        public override void AttachToProduct(IProduct[] products)
        {
            products.Each(p => p.AddSale(this));
        }

        public override decimal GetSale(decimal price)
        {
            return price * percentSale;
        }
    }

    public class Sale2 : Sale1
    {
        public Sale2(decimal sale) : base(sale) { }

        public override void AttachToProduct(IProduct[] products)
        {
            var product = products.Where(p => p.GetOrder() > GetOrder()).OrderBy(o => o).FirstOrDefault();
            if (product != null)
                product.AddSale(this);
        }
    }

    public 
    class Sale3<T> : Sale
        where T : IProduct
    {
        readonly int nth;
        readonly decimal dolarSale;

        public Sale3(int nth, decimal dolarSale)
        {
            this.nth = nth;
            this.dolarSale = dolarSale;
        }

        public override void AttachToProduct(IProduct[] products)
        {
            var typedProducts = products.Where(p => typeof(T).IsAssignableFrom(p.GetType())).OrderBy(p => p.GetOrder());
            if (typedProducts.Count() < nth)
                return;
            typedProducts.Skip(nth - 1).First().AddSale(this);
        }

        public override decimal GetSale(decimal price)
        {
            return price - Math.Min(price, dolarSale);
        }
    }
}

using System;
using Xunit;
using CodingExam.Classlib;

namespace CodingExam.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void TestSale1()
        {
            var cart = new Cart();
            var sale1 = new Sale1(0.9m);
            var tshirt = new TShirt(10);

            cart.AddProduct(tshirt);
            cart.AddProduct(new Product(10));
            cart.AddSale(sale1);

            //Cart price should be $18
            Assert.Equal(18, cart.Calculate());
        }

        [Fact]
        public void TestSale2()
        {
            var cart = new Cart();
            var sale2 = new Sale2(0.5m);

            cart.AddSale(sale2);
            cart.AddProduct(new Product(10));

            //Cart price should be $5
            Assert.Equal(5, cart.Calculate());
        }

        [Fact]
        public void TestSale3()
        {
            var cart = new Cart();
            var sale3 = new Sale3<TShirt>(1, 11);
            var tshirt = new TShirt(10);

            cart.AddProduct(tshirt);
            cart.AddProduct(new Product(10));
            cart.AddSale(sale3);

            //Cart price should be $10
            Assert.Equal(10, cart.Calculate());
        }
    }
}

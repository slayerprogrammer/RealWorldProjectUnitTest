using Microsoft.AspNetCore.Mvc;
using Moq;
using RealWorldProjectUnitTest.Web.Controllers;
using RealWorldProjectUnitTest.Web.Models;
using RealWorldProjectUnitTest.Web.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RealWorldProjectUnitTest.Test.Products
{
    public class ProductControllerTest
    {
        //bir kere nesne örneği oluşturup değiştirmeyeceğimiz için private readonly diyoruz
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;
        private List<Product> products;

        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepo.Object);
            products = new List<Product>()
            {
                new Product{Id =1, Name = "Kalem", Color = "Kırmızı", Price = 100, Stock = 10, Description = "Güzel Kalem"},
                new Product{Id =2, Name = "Defter", Color = "Beyaz", Price = 300, Stock = 20, Description = "Çizgili Kalem"}
            };
        }

        //Index Metodunu Test Ediyoruz
        //İsimlendirme 3 aşamadan oluşuyordu hatırlarsanız hangi metot, tipi, dönüşü
        //IsType ile tipine bakıyoruz
        //Index metodu return View dönüyor.
        //herhangi bir parametre almadığı için fact kullanıyoruz
        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        //getall metodunu mocklayalım
        //mocklama taklit veritabanına bağlanmak yerine memorydeki datamızı alarak taklit ediyoruz.
        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

            var result = await _controller.Index();
            //tip kontrol IsType
            var viewResult = Assert.IsType<ViewResult>(result);
            
            //ViewResult dönen modelini kontrol ettik
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            //gelen datanın sayısını kontrol ediyoruz
            Assert.Equal<int>(2, productList.Count());
        }


    }
}

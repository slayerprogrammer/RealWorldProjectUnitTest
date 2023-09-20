using Microsoft.AspNetCore.Mvc;
using Moq;
using NuGet.ContentModel;
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

        #region Index Method
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
        #endregion

        #region Detail Method
        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Details(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }


        [Fact]
        public async void Details_IdInValid_ReturnNotFound()
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetByIdAsync(0)).ReturnsAsync(product);

            var result = await _controller.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        //id si 1 olanı bulalım.
        [Theory]
        [InlineData(1)]

        public async void Details_ValidId_ReturnProduct(int productId)
        {
            Product product = products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.Details(productId);

            //dönüş tipi kontrol ediliyor
            var viewResult = Assert.IsType<ViewResult>(result);

            //product modeli geliyor mu kontrol ediliyor
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            //bana gelen product ile dönen product nesnesi aynı mı kontrol edelim
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }
        #endregion

        #region Create Method
        //override da 2 durum söz konusu ilki product alma durumu
        //ikincisi değer almama durumu product alırsa post işlemi gerçekleşir
        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void CreatePOST_InValidModelState_ReturnView()
        {
            //bir hata oluşturuyoruz.
            _controller.ModelState.AddModelError("Name", "Name Alanı Gereklidir");

            //yukarıdaki products listemizdeki ilk ürünü çekiyoruz
            var result = await _controller.Create(products.First());

            //result tipini kontrol edelim.
            var viewResult = Assert.IsType<ViewResult>(result);

            //view result ın dönen modelini kontrol edelim.
            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(products.First());

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_CreateMethodExecute()
        {
            Product newProduct = null;

            //mockRepo da çalıştırdığımızda içindeki methot çalışırken ekstra çalışacak 
            _mockRepo.Setup(x => x.CreateAsync(It.IsAny<Product>())).Callback<Product>(y => newProduct = y);

            //burada create methodumuzu çağırdığımızda setup çalıştığında
            //products listemizdeki ilk değeri newProduct a aktarıp çalıştırmaktadır.
            var result = await _controller.Create(products.First());

            //create methodun çalışıp çalışmadığını verify ile kontrol ediyoruz.
            _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Once);

            //gönderilen değerlerin karşılaştırılması yapılmaktadır.
            Assert.Equal(products.First().Id, newProduct.Id);

        }

        [Fact]
        public async void CreatePOST_InValidModelState_NeverCrateMethodExecute()
        {
            _controller.ModelState.AddModelError("Name", "Zorunlu Alan Mevcut");

            var result = await _controller.Create(products.First());

            _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Never);
        }

        #endregion

        #region Edit GET Method

        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Edit(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(3)]
        public async void Edit_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.Edit(productId);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecutes_ReturnProduct(int productId)
        {
            var product = products.FirstOrDefault(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.Edit(productId);

            //direkt tipini kontrol eder
            var viewResult = Assert.IsType<ViewResult>(result);

            //referance verilip verilemediğini test eder.
            //burada miras alınabilme durumuna kadar kontrol edebilirsiniz
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        #endregion

        #region Edit POST Method

        [Theory]
        [InlineData(1)]
        public async void EditPOST_IsNotEqualProduct_ReturnNotFound(int productId)
        {
            var result = await _controller.Edit(2, products.FirstOrDefault(x => x.Id == productId));

            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void EditPOST_IsValidModelState_ReturnView(int productId)
        {
            _controller.ModelState.AddModelError("Name", "Name alanı boş bırakılamaz");
            var result = await _controller.Edit(productId, products.FirstOrDefault(x => x.Id == productId));

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewResultProduct = Assert.IsType<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void EditPOST_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {
            var result = await _controller.Edit(productId, products.FirstOrDefault(x => x.Id == productId));
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public async void EditPOST_ValidModelState_UpdateMethodExecute(int productId)
        {
            var product = products.FirstOrDefault(x => x.Id == productId);
            _mockRepo.Setup(x => x.UpdateAsync(product));
            await _controller.Edit(productId, product);

            _mockRepo.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);
        }

        #endregion

        #region Delete GET Method

        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]

        public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x=>x.GetByIdAsync(productId)).ReturnsAsync(product);
            var result = await _controller.Delete(productId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(2)]
        public async void Delete_ActionExecutes_ReturnProduct(int productId)
        {
            var product = products.FirstOrDefault(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.Delete(productId);

            //direkt tipini kontrol eder
            var viewResult = Assert.IsType<ViewResult>(result);

            //referance verilip verilemediğini test eder.
            //burada miras alınabilme durumuna kadar kontrol edebilirsiniz
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        #endregion

        #region Delete Post Method
        [Theory]
        [InlineData(2)]
        public async void DeleteConfirmedPOST_ActionExecutes_ReturnRedirectToIndexAction(int productId)
        {
            var result = await _controller.DeleteConfirmed(productId);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }


        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmedPOST_ActionExecutes_DeleteMethodExecute(int productId)
        {
            var product = products.FirstOrDefault(x => x.Id == productId);
            _mockRepo.Setup(x => x.DeleteAsync(product));
            await _controller.DeleteConfirmed(productId);

            _mockRepo.Verify(x => x.DeleteAsync(It.IsAny<Product>()), Times.Once);
        }
        #endregion

        #region ProductExists Method

        [Theory]
        [InlineData(0)]
        public async void ProductExists_IdIsNotEqualsProduct_ReturnBoolenFalse(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
            var result = await _controller.ProductExists(productId);

            Assert.Equal(false,result);
        }

        [Theory]
        [InlineData(1)]
        public async void ProductExists_IdIsNotEqualsProduct_ReturnBoolenTrue(int productId)
        {
            var product = products.FirstOrDefault(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
            var result = await _controller.ProductExists(productId);

            Assert.Equal(true, result);
        }

        #endregion

    }
}

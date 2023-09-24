using Microsoft.AspNetCore.Mvc;
using Moq;
using RealWorldProjectUnitTest.Web.Controllers;
using RealWorldProjectUnitTest.Web.Helpers;
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
    public class ProductApiControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsApiController _controller;
        private readonly Helper _helper;

        private List<Product> products;

        public ProductApiControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsApiController(_mockRepo.Object);
            _helper = new Helper();
            products = new List<Product>()
            {
                new Product{Id =1, Name = "Kalem", Color = "Kırmızı", Price = 100, Stock = 10, Description = "Güzel Kalem"},
                new Product{Id =2, Name = "Defter", Color = "Beyaz", Price = 300, Stock = 20, Description = "Çizgili Kalem"}
            };
        }

        [Theory]
        [InlineData(2,3,5)]
        public void Add_SampleCalculator_ReturnTotal(int a, int b, int total)
        {
            var result = _helper.add(a, b);
            Assert.Equal(result, total);
        }


        [Fact]
        public async void GetProductAsync_ActionExecutes_ReturnOkResultWithProduct()
        {
            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

            var result = await _controller.GetProductAsync().ConfigureAwait(false);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProduct = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal<int>(2, returnProduct.ToList().Count());
        }

        [Theory]
        [InlineData(0)]
        public async void GetProductByIdAsync_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.GetProductById(productId);

            Assert.IsType<NotFoundResult>(result);

        }

        [Theory]
        [InlineData(1)]

        public async void GetProductByIdAsync_IdValid_ReturnOkResult(int productId)
        {
            var product = products.FirstOrDefault(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.GetProductById(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsType<Product>(okResult.Value);

            Assert.Equal(productId, returnProduct.Id);
        }

        [Theory]
        [InlineData(1)]
        public async void PutProduct_IdIsNotEqualProduct_ReturnBadRequestResult(int productId)
        {
            var product = products.FirstOrDefault(x => x.Id == productId);

            var result = await _controller.PutProduct(2, product);

            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }


        [Theory]
        [InlineData(1)]
        public async void PutProduct_ActionExecutes_ReturnNoContent(int productId)
        {
            var product = products.FirstOrDefault(x => x.Id == productId);

            _mockRepo.Setup(x => x.UpdateAsync(product));

            var result = await _controller.PutProduct(productId, product);

            _mockRepo.Verify(x => x.UpdateAsync(product), Times.Once);

            Assert.IsType<NoContentResult>(result);

        }

        [Fact]
        public async void PostProduct_ActionExecutes_ReturnCreateAtAction()
        {
            var product = products.First();

            //işlemin tamamlandığını belirtmek için task.completedtask tamamladığını bildirmek için kullanıyoruz.
            _mockRepo.Setup(x => x.CreateAsync(product)).Returns(Task.CompletedTask);

            var result = await _controller.PostProduct(product);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal("GetProductAsync", createdAtActionResult.ActionName);

            _mockRepo.Verify(x => x.CreateAsync(product), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        public async void DeleteProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            //burada farklı bir senaryo deniyoruz önceki yazımızda hep IActionResult dönerdi
            //bu sefer ActionResult dönüyoruz
            var resultNotFound = await _controller.DeleteProduct(productId);

            //eğer geriye bir sınıf dönüyor actionresult dönüyorsa mutlaka ilgili datanın result property üzerinden kontrol ediyoruz
            //eğer bir interface dönüyorsa (bizim tarafta IActionResult) result yazmamıza gerek kalmayacaktı.
            Assert.IsType<NotFoundResult>(resultNotFound.Result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteProduct_ActionExecutes_ReturnNoContent(int productId)
        {
            var product = products.FirstOrDefault(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockRepo.Setup(x => x.DeleteAsync(product));

            var result = await _controller.DeleteProduct(productId);

            _mockRepo.Verify(x=>x.DeleteAsync(product), Times.Once);

            Assert.IsType<NoContentResult>(result.Result);
        }
    }
}

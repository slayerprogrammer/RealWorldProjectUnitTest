using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealWorldProjectUnitTest.Web.Controllers;
using RealWorldProjectUnitTest.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RealWorldProjectUnitTest.Test.Products
{
    public class ProductMultiControllerTestWithInMemory : ProductMultiController
    {
        public ProductMultiControllerTestWithInMemory()
        {
            SetContextOptions(new DbContextOptionsBuilder<RealWorldProjectContext>()
                    .UseInMemoryDatabase("RealWorldProjectUnitTestInMemory").Options);
        }

        [Fact]
        public async Task CreatePOST_ModelValidProduct_ReturnsRedirectToActionWithSaveProduct()
        {
            var newProduct = new Product
            {
                Name = "Kalem 30",
                Price = 100,
                Color = "Kırmızı",
                Stock  = 50,
                Description = "Kalem Kaliteli"
            };

            using (var context = new RealWorldProjectContext(_dbContextOptions))
            {
                var category = context.Category.FirstOrDefault(x => x.Name == "Kalemler");
                newProduct.CategoryId = category.Id;

                //repository olsa idi yapılacak örnek
                //var repository = new repository<Product>()
                var controller = new ProductsMultiController(context);
                var result = await controller.Create(newProduct);

                var redirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirect.ActionName);
            }

            //ayrı contextlerde yapıyorum o işlem bitmeden diğeri geçerken farklı veriyi almasın
            using (var context = new RealWorldProjectContext(_dbContextOptions))
            {
                var product = context.Products.FirstOrDefault(x => x.Name == newProduct.Name);
                Assert.Equal(newProduct.Name, product.Name);
            }
        }
    }
}

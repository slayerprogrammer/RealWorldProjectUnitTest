﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
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
    public class ProductMultiControllerTestWithInSqlDb : ProductMultiControllerBase
    {
        public ProductMultiControllerTestWithInSqlDb()
        {
            var sqlCon = @"Data Source=.;Initial Catalog=RealWorldProjectUnitTest;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;";

            SetContextOptions(new DbContextOptionsBuilder<RealWorldProjectContext>().UseSqlServer(sqlCon).Options);
        }

        [Fact]
        public async Task CreatePOST_ModelValidProduct_ReturnsRedirectToActionWithSaveProduct()
        {
            var newProduct = new Product
            {
                Name = "Simli Yeşil Kalem",
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

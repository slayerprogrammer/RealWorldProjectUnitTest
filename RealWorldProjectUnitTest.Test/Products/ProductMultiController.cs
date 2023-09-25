using Microsoft.EntityFrameworkCore;
using RealWorldProjectUnitTest.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldProjectUnitTest.Test.Products
{
    public class ProductMultiController
    {
        //miras alanlar erişebilsin protected
        protected DbContextOptions<RealWorldProjectContext> _dbContextOptions { get; private set; }

        public void SetContextOptions(DbContextOptions<RealWorldProjectContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
            Seed();
        }

        public void Seed()
        {
            using (RealWorldProjectContext context = new RealWorldProjectContext(_dbContextOptions))
            {
                //silme işlemi
                context.Database.EnsureDeleted();

                //oluşturma işlemi
                context.Database.EnsureCreated();

                context.Category.Add(new Category() { Name = "Kalemler" });
                context.Category.Add(new Category() { Name = "Silgiler" });
                context.Category.Add(new Category() { Name = "Defterler" });
                context.SaveChanges();

                context.Products.Add(new Product() { CategoryId = 1, Name = "Faber Castel Kalem", Color = "Kırmızı", Description = "Kaliteli Güzel Kalem", Price = 100, Stock = 5});
                context.Products.Add(new Product() { CategoryId = 1, Name = "Faber Castel Kalem", Color = "Mavi", Description = "Kaliteli Güzel Kalem", Price = 100, Stock = 5});
                context.SaveChanges();

            }
        }
    }
}

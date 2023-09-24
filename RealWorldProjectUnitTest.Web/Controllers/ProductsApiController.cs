using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealWorldProjectUnitTest.Web.Models;
using RealWorldProjectUnitTest.Web.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RealWorldProjectUnitTest.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly IRepository<Product> _context;

        public ProductsApiController(IRepository<Product> context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductAsync()
        {
            var products = await _context.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.GetByIdAsync(id);

            if(product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> PutProduct(int id, Product model)
        {
            if (id != model.Id)
                return BadRequest();

            await _context.UpdateAsync(model);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct( Product model)
        {
            await _context.CreateAsync(model);
            return CreatedAtAction("GetProductAsync", new {id = model.Id}, model);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            await _context.DeleteAsync(product);

            return NoContent();
        }

        public bool ProductExists(int id)
        {
            var product = _context.GetByIdAsync(id).Result;

            if (product == null)
                return false;
            else
                return true;
        }

    }
}

using FullStackAPi.Data;
using FullStackAPi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullStackAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly FullStackDbContext _fullStackDbContext;
        public ProductController(FullStackDbContext fullStackDbContext)
        {
            _fullStackDbContext = fullStackDbContext;
        }

        [HttpGet]
      
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _fullStackDbContext.Products.ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            //product.Id = Guid.NewGuid();

            var addproduct = await _fullStackDbContext.Products.AddAsync(product);
                await _fullStackDbContext.SaveChangesAsync();
           
            return Ok(product);
        }

        [HttpDelete]
        [Route("deleteproduct/{id:int}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            var product = await _fullStackDbContext.Products.FindAsync(id);
            if (product == null) { return NotFound(); }
            _fullStackDbContext.Products.Remove(product);
            await _fullStackDbContext.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPut]
        [Route("updateproduct/{id:int}")]
        public async Task<IActionResult> UpdateProduct([FromRoute]int id, Product updateProductRequested)
        {
            var product = await _fullStackDbContext.Products.FindAsync(id);
            if (product == null) { return NotFound(); }
            product.Title = updateProductRequested.Title;
            product.Description = updateProductRequested.Description;
            product.Price = updateProductRequested.Price;
           
            product.Image = updateProductRequested.Image;
          
           
            await _fullStackDbContext.SaveChangesAsync();
            return Ok(product);
        }


        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
            var product = await _fullStackDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) { return NotFound(); }
            return Ok(product);
        }
        [HttpGet]
        [Route("productbycatid/{id}")]
        public async Task<IActionResult> GetProductByCategoryId([FromRoute] string id)
        {
            var products = await _fullStackDbContext.Products.ToListAsync();
            var filterproducts = products.Where(p => p.Category == id).OrderByDescending(p => p.Id);

            return Ok(filterproducts);
        }

        [HttpGet]
        [Route("category")]
        public async Task<IActionResult> GetAllCategories()
        {
            //var categories = await _fullStackDbContext.Categories.ToListAsync();
            var categories = await _fullStackDbContext.Categories.Select(x=>x.CategoryName).ToListAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Route("category/{id}")]
        public async Task<IActionResult> GetCategory([FromRoute] string id)
        {
            var category = await _fullStackDbContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (category == null) { return NotFound(); }
            return Ok(category);
        }

    }
}

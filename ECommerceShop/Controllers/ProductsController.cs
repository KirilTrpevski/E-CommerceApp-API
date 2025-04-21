using ECommerceShop;
using ECommerceShop.Data;
using ECommerceShop.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    public class ProductsController(DataContext context, RecommendationService _recommendationService) : ControllerBase
    {

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(
                                                        [FromForm] string name,
                                                         [FromForm] string description,
                                                         [FromForm] decimal price,
                                                         [FromForm] string category,
                                                         [FromForm] int stock,
                                                         [FromForm] IFormFile image)

        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("Image is required.");
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Price = price,
                Category = category,
                Stock = stock
            };

            // Convert the image to a byte array and store it in the product
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                product.Image = memoryStream.ToArray();
            }

            // Add the product to the database
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Return the created product
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await context.Products.ToListAsync();
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost("product-interaction")]
        public IActionResult TrackProductInteraction([FromBody] ProductInteractionsDto interactionDto)
        {
            var existingInteraction = context.ProductInteractions
        .FirstOrDefault(pi => pi.UserId == interactionDto.UserId && pi.ProductId == interactionDto.ProductId);
            // Save the interaction in the database
            if (existingInteraction == null)
            {
                var interaction = new ProductInteraction
                {
                    UserId = interactionDto.UserId,
                    ProductId = interactionDto.ProductId,
                    InteractionType = interactionDto.InteractionType, // e.g., 1 for purchase, 0 for view
                    InteractionDate = DateTime.UtcNow
                };

                context.ProductInteractions.Add(interaction);
                context.SaveChanges();

                // return Ok();
            }
            return Ok();

        }

        [HttpGet("recommendations/{userId}")]
        public async Task<IActionResult> GetRecommendations(Guid userId)
        {
            var recommendedProducts = await _recommendationService.GetRecommendedProductsAsync(userId);
            return Ok(recommendedProducts);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(); // Product not found
            }

            // Remove the product from the database
            context.Products.Remove(product);
            await context.SaveChangesAsync();

            return NoContent(); // Successfully deleted, no content to return
        }

        // PUT api/products/{id}
        [EnableCors("AllowAllOrigins")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] string name, [FromForm] string description,
                                                       [FromForm] decimal price, [FromForm] string category,
                                                       [FromForm] int stock, [FromForm] IFormFile? image)
        {
            // Find the product by ID
            var product = await context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(); // Product not found
            }

            // Update the product fields
            product.Name = name;
            product.Description = description;
            product.Price = price;
            product.Category = category;
            product.Stock = stock;

            // If a new image is provided, update it
            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    product.Image = memoryStream.ToArray();
                }
            }

            // Save the updated product to the database
            context.Products.Update(product);
            await context.SaveChangesAsync();

            return NoContent(); // Successfully updated, no content to return
        }

    }
}
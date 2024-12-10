using Microsoft.AspNetCore.Mvc;
using Sharebook.Core.Models;
using Sharebook.Services.Interfaces;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace Sharebook.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public readonly IProductService _productService;
        private readonly IConnectionMultiplexer _redis;
        public const int ProductCacheTTL = 15;

        public ProductsController(IProductService productService,
            IConnectionMultiplexer redis)
        {
            _productService = productService;
            _redis = redis;
        }

        /// <summary>
        /// Get the list of product
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all products", Description = "Returns a list of all products.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductList()
        {
            try
            {
                var db = _redis.GetDatabase();
                var cachedProductList = await db.StringGetAsync("products:list");

                if (!cachedProductList.IsNullOrEmpty)
                {
                    Console.WriteLine("Cached list 2");
                    var deserializedProductList = JsonSerializer.Deserialize<List<Product>>(cachedProductList);
                    return Ok(deserializedProductList);
                }

                var productList = await _productService.GetAllProducts();
                if (productList.Any())
                {
                    Console.WriteLine("Database list");
                    await db.StringSetAsync("products:list", JsonSerializer.Serialize(productList), TimeSpan.FromSeconds(ProductCacheTTL));
                    return Ok(productList);
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return Ok();
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet("{productId}")]
        [SwaggerOperation(Summary = "Get product by id", Description = "Returns a product.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductById(int productId)
        {
            try
            {
                var db = _redis.GetDatabase();

                var cachedProduct = await db.StringGetAsync($"product:{productId}");
                if (!cachedProduct.IsNullOrEmpty)
                {
                    var deserializedProduct = JsonSerializer.Deserialize<Product>(cachedProduct);
                    return Ok(deserializedProduct);
                }

                var product = await _productService.GetProductById(productId);

                if (product != null)
                {
                    await db.StringSetAsync($"product:{productId}", JsonSerializer.Serialize(product), TimeSpan.FromSeconds(ProductCacheTTL));
                    return Ok(product);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return Ok();
        }

        /// <summary>
        /// Add a new product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Add a product", Description = "Create a product.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            var isProductCreated = await _productService.CreateProduct(product);

            if (isProductCreated)
            {
                return Ok(isProductCreated);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Update the product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerOperation(Summary = "Update a product", Description = "Edit a existent product.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            if (product != null)
            {
                var isProductCreated = await _productService.UpdateProduct(product);
                if (isProductCreated)
                {
                    return Ok(isProductCreated);
                }
                return BadRequest();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete product by id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpDelete("{productId}")]
        [SwaggerOperation(Summary = "Remove a product", Description = "Remove a existent product by Id.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var isProductCreated = await _productService.DeleteProduct(productId);

            if (isProductCreated)
            {
                return Ok(isProductCreated);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}

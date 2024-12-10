using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Annotations;

namespace Sharebook.API.Controllers
{
    [Route("api/visitors")]
    [ApiController]
    public class VisitorController : Controller
    {
        private readonly IConnectionMultiplexer _redis;

        public VisitorController(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        /// <summary>
        /// Retorna o número aproximado de visitantes únicos de um produto.
        /// </summary>
        [HttpGet("product/{productId}")]
        [SwaggerOperation(Summary = "Get product visitors by product id", Description = "Returns a product visitors count.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUniqueVisitors(string productId)
        {
            try
            {
                string redisKey = $"product:{productId}:visitors";

                var db = _redis.GetDatabase();

                // Obtém a contagem aproximada de visitantes únicos
                long uniqueVisitorsCount = await db.HyperLogLogLengthAsync(redisKey);

                return Ok(new
                {
                    ProductId = productId,
                    UniqueVisitors = uniqueVisitorsCount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eita! Deu erro: {ex.Message}");
            }

            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Sharebook.API.Controllers
{
    [Route("api/locations")]
    [ApiController]
    public class LocationController : Controller
    {
        private readonly IConnectionMultiplexer _redis;

        public LocationController(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        /// <summary>
        /// Busca produtos próximos a uma localização geográfica.
        /// </summary>
        [HttpGet("products/nearby")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetNearbyProducts(double latitude, double longitude, double radiusInKm)
        {
            string redisKey = $"places:products";

            var db = _redis.GetDatabase();

            // Realiza uma consulta geoespacial no Redis
            var results = db.GeoRadius(
                redisKey,
                longitude,
                latitude,
                radiusInKm,
                GeoUnit.Kilometers);

            var nearbyProducts = results.Select(result => new
            {
                ProductId = result.Member.ToString(),
                Distance = result.Distance, // Distância ao ponto de origem
                Coordinates = result.Position
            });

            return Ok(nearbyProducts);
        }

        /// <summary>
        /// Calcula a distância entre dois produtos com base em seus IDs.
        /// </summary>
        [HttpGet("products/distance")]
        public IActionResult GetDistanceBetweenProducts(int productId1, int productId2)
        {
            string redisKey = $"places:products";

            var db = _redis.GetDatabase();

            var distance = db.GeoDistance(
                redisKey,
                productId1.ToString(),
                productId2.ToString(),
                GeoUnit.Kilometers);

            return Ok(new { DistanceInKm = distance?.ToString() });
        }
    }
}

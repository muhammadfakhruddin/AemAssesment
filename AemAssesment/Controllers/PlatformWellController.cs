using AemAssesment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AemAssesment.Controllers
{
    [ApiController]
    [Route("api")]
    public class PlatformWellController(
        DBContext db, 
        PlatformWellApiClient client, 
        ILogger<PlatformWellController> logger) : ControllerBase
    {
        // POST api/sync
        [HttpPost("sync")]
        public async Task<IActionResult>Sync(CancellationToken ct = default)
        {
            try
            {
                var root = await client.GetPlatformWells(ct);
                var (platforms, wells) = await PlatformWellServices.Import(db, root, ct);
                return Ok(new { platforms, wells });
            }
            catch(Exception ex) when(ex is HttpRequestException or JsonException)
            {
                logger.LogError(ex, "Failed to sync with external API");
                return StatusCode(StatusCodes.Status502BadGateway, new { error = "Failed to sync with external API." });
            }
        }
    }
}
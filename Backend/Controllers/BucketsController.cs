using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BucketsController(MinioService minioService) : ControllerBase
    {
        private readonly MinioService _minioService = minioService;

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<string>>> GetBuckets()
        {
            try
            {
                return Ok(await _minioService.ListBucketsAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

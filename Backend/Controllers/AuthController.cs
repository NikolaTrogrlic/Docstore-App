using Backend.Models.Requests;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(UserService userService, MinioService minio) : ControllerBase
    {
        private readonly UserService _userService = userService;
        private readonly MinioService _minioService = minio;

        [HttpPost("")]
        public ActionResult<string> Login(LoginRequest request)
        {
            try
            {
                var result = _userService.AuthenticateUser(request.Username, request.Password);
                if (result.StartsWith("ERR"))
                {
                    return Unauthorized(new {result});
                }
                else
                {
                    return Ok(new {token = result, username = request.Username, allowedBuckets = _minioService.GetAllowedBuckets()});
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

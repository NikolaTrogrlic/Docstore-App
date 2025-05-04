using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilesController(MinioService minioService, UserService userService) : ControllerBase
    {
        private readonly MinioService _minioService = minioService;
        private readonly UserService _userService = userService;


        [HttpGet("{bucketName}")]
        public async Task<ActionResult<IEnumerable<FileData>>> ListFiles(string bucketName, string? prefix)
        {
            try
            {
                if (!await _minioService.BucketExistsAsync(bucketName))
                {
                    return NotFound($"Bucket '{bucketName}' does not exist");
                }

                if (_userService.IsUser(HttpContext) == false)
                    return Unauthorized("User is not authenticated");

                if (string.IsNullOrEmpty(prefix))
                {
                    prefix = _userService.GetContextUsername(HttpContext) + "/";
                }

                var items = await _minioService.ListFilesAsync(bucketName, prefix, true);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{bucketName}/{username}/{fileName}")]
        public async Task<ActionResult> GetFile(string bucketName, string username, string fileName)
        {
            try
            {
                if (!await _minioService.BucketExistsAsync(bucketName))
                {
                    return NotFound($"Bucket '{bucketName}' does not exist");
                }

                if (_userService.IsUser(HttpContext) == false)
                    return Unauthorized("User is not authenticated");

                if (username.Equals(_userService.GetContextUsername(HttpContext)) == false
                                   && _userService.IsAdmin(HttpContext) == false)
                    return Forbid("Users can only upload files to their own folder");

                var objectName = GetObjectName(username, fileName);

                try
                {
                    var objectStat = await _minioService.GetObjectStatAsync(bucketName, objectName);
                    var stream = await _minioService.GetFileAsync(bucketName, objectName);
                    return File(stream, objectStat.ContentType, objectName);
                }
                catch (Minio.Exceptions.ObjectNotFoundException)
                {
                    return NotFound($"File '{objectName}' does not exist in bucket '{bucketName}'");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{bucketName}/{username}/{fileName}")]
        public async Task<ActionResult> UploadFile(string bucketName, string username,string fileName, IFormFile file)
        {
            try
            {
                //Moze se jos nadograditi da request prvo vrati informaciju da li file već postoji pa overwrite-a ako postoji

                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file was uploaded");
                }

                if (_userService.IsUser(HttpContext) == false)
                    return Unauthorized("User is not authenticated");

                if (username.Equals(_userService.GetContextUsername(HttpContext)) == false 
                    && _userService.IsAdmin(HttpContext) == false)
                    return Forbid("Users can only upload files to their own folder");

                if (!await _minioService.BucketExistsAsync(bucketName))
                {
                    await _minioService.CreateBucketAsync(bucketName);
                }

                string objectName = GetObjectName(username, fileName);

                using (var stream = file.OpenReadStream())
                {
                    await _minioService.UploadFileAsync(
                        bucketName,
                        objectName,
                        stream,
                        file.Length,
                        file.ContentType);
                }

                return Ok(new { message = $"File '{objectName}' uploaded successfully to bucket '{bucketName}'" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{bucketName}/{username}/{fileName}")]
        public async Task<ActionResult> DeleteFile(string bucketName, string username, string fileName)
        {
            try
            {
                if (!await _minioService.BucketExistsAsync(bucketName))
                {
                    return NotFound($"Bucket '{bucketName}' does not exist");
                }


                if (_userService.IsUser(HttpContext) == false)
                    return Unauthorized("User is not authenticated");

                if (username.Equals(_userService.GetContextUsername(HttpContext)) == false
                    && _userService.IsAdmin(HttpContext) == false)
                    return Forbid("Users can only delete files to their own folder");

                string objectName = GetObjectName(username, fileName);

                //TODO: Log

                await _minioService.DeleteFileAsync(bucketName, objectName);
                return Ok(new { message = $"File '{objectName}' deleted successfully from bucket '{bucketName}'" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GetObjectName(string username, string fileName) => $"{username}/{fileName}";
    }
}

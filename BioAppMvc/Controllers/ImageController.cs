using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BioAppMvc.Services;

namespace BioAppMvc.Controllers
{
    public class ImageController : Controller
    {
        private readonly BlobService _blobService;

        public ImageController(BlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var fileName = file.FileName;
            var contentType = file.ContentType;

            using (var stream = file.OpenReadStream())
            {
                await _blobService.UploadFileAsync(stream, fileName, contentType);
            }

            var imageUrl = _blobService.GetBlobUrl(fileName);

            return Ok(new { url = imageUrl });
        }
    }
}

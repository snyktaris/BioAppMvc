using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
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

            var containerName = "images";
            var fileName = file.FileName;

            using (var stream = file.OpenReadStream())
            {
                await _blobService.UploadFileAsync(stream, containerName, fileName);
            }

            var imageUrl = _blobService.GetBlobUrl(file.FileName);

            dynamic data = JsonConvert.DeserializeObject(imageUrl) ?? new object();
            return Ok(new { url = imageUrl });
        }
    }
}

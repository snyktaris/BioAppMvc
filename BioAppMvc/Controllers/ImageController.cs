using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using BlobService.Services; // Adjust this to where BlobService is
using System.IO;

namespace BioAppMvc.Controllers
{
    public class ImageController : Controller
    {
        private readonly BlobService _blobService;

        public ImageController(BlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var files = await _blobService.ListFilesAsync();
            return View(files);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var fileUrl = await _blobService.UploadFileAsync(stream, file.FileName, file.ContentType);
                ViewBag.Message = "File uploaded successfully: " + fileUrl;
            }
            else
            {
                ViewBag.Message = "Please select a valid file.";
            }

            var files = await _blobService.ListFilesAsync();
            return View("Index", files);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string fileName)
        {
            var result = await _blobService.DeleteFileAsync(fileName);

            ViewBag.Message = result
                ? $"File '{fileName}' deleted successfully."
                : $"File '{fileName}' not found or already deleted.";

            var files = await _blobService.ListFilesAsync();
            return View("Index", files);
        }
    }
}
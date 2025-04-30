using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class ImageController : Controller
{
    private readonly BlobService _blobService;

    public ImageController(BlobService blobService)
    {
        _blobService = blobService;
    }

    // Upload file
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file != null)
        {
            var fileUrl = await _blobService.UploadFileAsync(file);
            ViewBag.FileUrl = fileUrl; // Display URL to the user
        }
        return View();
    }

    // Get file URL (for API)
    [HttpGet]
    public IActionResult Get(string fileName)
    {
        var fileUrl = _blobService.GetBlobUrl(fileName);
        return Redirect(fileUrl);
    }
}

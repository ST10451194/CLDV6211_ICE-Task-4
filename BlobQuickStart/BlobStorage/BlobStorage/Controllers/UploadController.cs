using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

public class UploadController : Controller
{
    private readonly string _connectionString;
    private readonly string _containerName;

    public UploadController(IConfiguration configuration)
    {
        _connectionString = configuration["AzureBlobStorage:ConnectionString"];
        _containerName = configuration["AzureBlobStorage:ContainerName"];
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ViewBag.Message = "No file selected";
            return View("Index");
        }

        // Create blob client
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(file.FileName);

        // Upload the file
        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);

        // Pass the URL to the view
        ViewBag.ImageUrl = blobClient.Uri.ToString();
        ViewBag.Message = "Upload successful!";
        return View("Index");
    }
}

using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileUploaderApp.Models;

namespace FileUploaderApp.Controllers
{
    public class FileController : Controller
    {
        private readonly BlobServiceClient _blobServiceClient;

        public FileController(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        // Index method to fetch uploaded files
        public async Task<IActionResult> Index()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("uploads");
            var blobs = containerClient.GetBlobs(); // This returns a Pageable<BlobItem>

            // Create a list to hold the file model data
            var fileList = new List<FileModel>();

            // Iterate through the blobs using a regular foreach loop
            foreach (var blob in blobs)
            {
                // Asynchronously retrieve the blob client URL
                var blobClient = containerClient.GetBlobClient(blob.Name);

                var fileModel = new FileModel
                {
                    FileName = blob.Name,
                    BlobUrl = blobClient.Uri.ToString()
                };
                fileList.Add(fileModel);
            }

            // Pass the file list to the view
            return View(fileList);
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(FileUploadModel model)
        {
            if (ModelState.IsValid && model.File != null)
            {
                // Create a container client for the "uploads" container
                var containerClient = _blobServiceClient.GetBlobContainerClient("uploads");
                await containerClient.CreateIfNotExistsAsync();

                // Create a blob client for the uploaded file
                var blobClient = containerClient.GetBlobClient(model.File.FileName);

                // Set the content type based on the uploaded file
                var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
                {
                    ContentType = model.File.ContentType
                };

                // Upload the file to the blob storage with the specified content type
                using (var stream = model.File.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                    {
                        HttpHeaders = blobHttpHeaders
                    });
                }

                // Redirect to Index after upload
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}

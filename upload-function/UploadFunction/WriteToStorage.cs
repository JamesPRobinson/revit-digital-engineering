using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace UploadFunction
{
    public static class WriteToStorage
    {
        [FunctionName("WriteToStorage")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        [Blob("revit-tool-images/{rand-guid}.png", FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")] CloudBlockBlob outputBlob,
        ILogger log)
        {
            var imageBase64String = req.Form["image"];
            var imageBuffer = Convert.FromBase64String(imageBase64String);
            using (var stream = new MemoryStream(imageBuffer))
            {
                await outputBlob.UploadFromStreamAsync(stream);
            }
            return new OkObjectResult(outputBlob.Uri);
        }
    }s
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;


namespace WebAplicationTestPerformance.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        //UPLOAD: Throughput HTTPS + Memory: 300 MB/s
        //http://weblogs.thinktecture.com/pawel/2017/03/aspnet-core-webapi-performance.html

        //DOWNLOAD: Throughput of 200 MB/s.
        //http://weblogs.thinktecture.com/pawel/2017/03/aspnet-core-webapi-performance.html
        [HttpPost("UploadMultipartUsingReader")]
        public async Task<IActionResult> UploadMultipartUsingReader()
        {

            
            var boundary = GetBoundary(Request.ContentType);
            var reader = new MultipartReader(boundary, Request.Body, 80 * 1024);
             
            var valuesByKey = new Dictionary<string, string>();
            //MultipartSection section;                                              

            var t = Request.Form.Files[0];

            var section = await reader.ReadNextSectionAsync();

            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                var contentDispo = section.GetContentDispositionHeader();

                if (contentDispo.IsFileDisposition())
                {
                    var fileSection = section.AsFileSection();
                    var bufferSize = 32 * 1024;
                    await Helpers.ReadStream(fileSection.FileStream, bufferSize);
                }
                else if (contentDispo.IsFormDisposition())
                {
                    var formSection = section.AsFormDataSection();
                    var value = await formSection.GetValueAsync();
                    valuesByKey.Add(formSection.Name, value);
                }
            }

            return Ok();
        }

        private static string GetBoundary(string contentType)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));

            var elements = contentType.Split(' ');
            var element = elements.First(entry => entry.StartsWith("boundary=", StringComparison.Ordinal));
            var boundary = element.Substring("boundary=".Length);

            boundary = HeaderUtilities.RemoveQuotes(boundary);

            return boundary;
        }

    }

    public static class Helpers
    {
        public static void PrintDuration(string methodName, int contentLength, TimeSpan duration)
        {
            var sizeInMb = contentLength / 1024 / 1024;
            Console.WriteLine($"{methodName} of {sizeInMb} MB took {duration}. Speed = {sizeInMb / duration.TotalSeconds:F} MB/s");
        }

        public static byte[] GetRandomBytes(int sizeInMb)
        {
            var size = sizeInMb * 1024 * 1024;
            var bytes = new byte[size];
            var random = new Random(0);
            random.NextBytes(bytes);

            return bytes;
        }

        public static async Task<int> ReadStream(Stream stream, int bufferSize)
        {
            var buffer = new byte[bufferSize];

            int bytesRead;
            int totalBytes = 0;

            do
            {
                bytesRead = await stream.ReadAsync(buffer, 0, bufferSize);
                totalBytes += bytesRead;
            } while (bytesRead > 0);
            return totalBytes;
        }
    }
}

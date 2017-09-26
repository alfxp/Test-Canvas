using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAplicationTestPerformance.Filters;
using WebAplicationTestPerformance.Helpers;
using Microsoft.AspNetCore.Hosting;

namespace WebAplicationTestPerformance.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {

        private readonly string _uploadFolder;
        public UploadController(IHostingEnvironment hostingEnvironment)
        {
            _uploadFolder = $"{hostingEnvironment.WebRootPath}\\Upload";
        }


        [Route("large")]
        [HttpPost]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> Large()
        {
            var fileCount = 0;
            await Request.StreamFile((file) =>
            {
                fileCount++;

                var t = file;

                return System.IO.File.Create($"c:\\1\\{file.FileName}"+".jpg");
            });

            return Ok(new { fileCount = fileCount });
        }

    }
}

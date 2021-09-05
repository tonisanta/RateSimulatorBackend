using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RateSimulator.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RateSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateController : ControllerBase
    {
        private readonly RateService rateService;

        public RateController(IOptions<ConfigurationPeriods> config) // canviar per IRateService
        {
            //this.rateService = new RateService();
            this.rateService = new RateService(config);
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("bueno no fa res");
        }

        [HttpPost]
        public async Task<IActionResult> ProcessFiles(List<IFormFile> files)
        {
            //System.Console.WriteLine(priceConfiguration);

            var saveFileTasks = new List<Task<string>>(files.Count);
            foreach (var file in files)
            {
                saveFileTasks.Add(SaveFile(file));
            }

            await Task.WhenAll(saveFileTasks);

            var pathFiles = new List<string>(files.Count);
            foreach (var task in saveFileTasks)
            {
                pathFiles.Add(task.Result);
            }

            var result = await rateService.ProcessFilesAsync(pathFiles);
            DeleteTempFiles(pathFiles);
            return Ok(result);
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            // alomillor ho hauria de fer una task perk sino deu ser el fil principal
            var filePath = Path.GetTempFileName();
            using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);
            stream.SetLength(stream.Length - 23);
            return filePath;
        }

        private static void DeleteTempFiles(IEnumerable<string> pathFiles)
        {
            foreach (var path in pathFiles)
            {
                System.IO.File.Delete(path);
            }
        }
    }
}

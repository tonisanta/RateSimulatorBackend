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

        public RateController(IOptions<ConfigurationPeriods> config)
        {
            this.rateService = new RateService(config);
        }

        [HttpGet]
        public IActionResult GetDefaultPrices()
        {
            return Ok(new PriceConfiguration());
        }

        [HttpPost]
        public async Task<IActionResult> ProcessFiles([FromForm] List<IFormFile> files, [FromForm] PriceConfiguration priceConfig)
        {
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

            try
            {
                var result = await rateService.ProcessFilesAsync(pathFiles, priceConfig);
                return Ok(result);
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
            finally
            {
                DeleteTempFiles(pathFiles);
            }         
        }

        private static async Task<string> SaveFile(IFormFile file)
        {
            var filePath = Path.GetTempFileName();
            using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);
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

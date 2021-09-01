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


        //[HttpPost]
        //public async Task<IActionResult> ProcessFiles(IFormFile file)
        //{
        //    rateService.

        //    return Ok(file.FileName);
        //}

        [HttpPost]
        public async Task<IActionResult> ProcessFiles(List<IFormFile> files)
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

            var result = await rateService.ProcessFilesAsync(pathFiles);

            //using var reader = new StreamReader(filePath);
            //using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            return Ok(result);
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var filePath = Path.GetTempFileName();
            using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);
            return filePath;
        }
    }
}

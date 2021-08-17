using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductImporter.Helpers;
using ProductImporter.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProductImporter.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            this.productService = productService;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("products")]
        public async Task<IActionResult> Products()
        {
            var productList =await productService.ListProducts();
            return View(new ProductListViewModel { Products = productList });
        }

        [HttpPost("upload")]
        //[Route]
        public async Task<IActionResult> UpdateProducts(IFormFile file)
        {
            if (file == null)
                return BadRequest("Yanlış istek atıldı.");

            if (file.ContentType != "application/vnd.ms-excel" && file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                return BadRequest("Sadece XLS ve XLSX uzantılı Excel dosyaları ile ürün güncelleme yapabilirsiniz.");

            if (file.Length == 0)
                return BadRequest("Ürün bulunamadı");
            var processResult = new ExcelProcessResult();
            using (var stream = file.OpenReadStream())
            {
                processResult = ExcelHelper.Process(stream);
            }

            if (processResult.Succeed)
            {
                var productImportResult = await productService.BulkWrite(processResult.ProceedRequests);

                return Ok(new
                {
                    ImportResults = productImportResult,
                    ProcessFaults = processResult.FailedRequests
                });
            }

            return BadRequest(processResult.ErrorMessage);
        }

        [HttpGet("download-template")]
        //[Route]
        public async Task<IActionResult> DownloadTemplate(bool xlsTemplate = false)
        {
            var extension = xlsTemplate ? "xls" : "xlsx";
            var contentType = xlsTemplate ? "application/vnd.ms-excel" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var filePath = Path.Combine("files", $"ProductUploadTemplate.{extension}");
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, contentType, $"Ürün yükleme şablonu.{extension}");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

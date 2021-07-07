using BFNet_2021_StorageCorrection.Models.Entities;
using BFNet_2021_StorageCorrection.Models.Views;
using BFNet_2021_StorageCorrection.Repositories;
using BFNet_2021_StorageCorrection.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BFNet_2021_StorageCorrection.Controllers
{
    public class ImageController : Controller
    {
        private ImageService _ImageService;
        public ImageController(ImageService imageService)
        {
            _ImageService = imageService;
        }

        public IActionResult List()
        {
            ImageListVM model = new ImageListVM();
            model.AddUris(_ImageService.ReadAll());

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create([FromServices] ProductRepository repository, IFormFile file)
        {
            Stream fileStream = file.OpenReadStream();
            byte[] bytes = new byte[file.Length];
            fileStream.Read(bytes);

            string uri = _ImageService.Upload(file.FileName, bytes, file.ContentType);

            Product product = new Product();
            product.PartitionKey = "user";
            product.RowKey = "1";
            product.Name = "Test";
            product.IsActive = true;
            product.IsPromoted = false;
            product.Price = 5.92F;

            repository.Create(product);
            repository.AddImageToProduct(product, uri);

            ViewBag.create = uri;

            return RedirectToAction("List");
        }
    }
}

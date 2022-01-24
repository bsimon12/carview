using CarViewer.Data.Services.Contracts;
using CarViewer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CarViewer.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly ICarDataService _carDataService;

        public HomeController(ILogger<HomeController> logger, ICarDataService carDataService) {
            _logger = logger;
            _carDataService = carDataService;
        }

        public void Seed() {
            _carDataService.SeedDemoData();
        }

        public IActionResult Index() {
            _logger.LogInformation("Retrieving all cars");
            var cars = _carDataService
                .GetAllMakeAndModel()
                .Select(car => 
                    new CarViewModel {
                        VIN = car.VIN,
                        Make = car.Model.Manufacturer?.Name,
                        Model = car.Model.Name,
                        Mileage = car.Mileage,
                        Year = car.Year
                    }
            );

            return View(cars);
        }

        public new IActionResult NotFound() {
            return View(new NotFoundViewModel());
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            _logger.LogError("An error occurred while processing request with id {RequestId}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
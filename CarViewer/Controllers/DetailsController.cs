using CarViewer.Data.Exceptions;
using CarViewer.Data.Services.Contracts;
using CarViewer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarViewer.Controllers {
    public class DetailsController : Controller {
        private readonly ILogger<DetailsController> _logger;
        private readonly ICarDataService _carDataService;
        public DetailsController(ILogger<DetailsController> logger, ICarDataService carDataService) {
            _logger = logger;
            _carDataService = carDataService;
        }

        [Route("Details")]
        public IActionResult Details(string vin) {
            try {
                var car = _carDataService.FindByVIN(vin);

                if (car == null) {
                    _logger.LogWarning("Attempted to retrieve details for car with VIN that was not found with vin: {vin}", vin);
                    return RedirectToAction("NotFound", "Home");
                }

                _logger.LogInformation("Retrieved car details for car: {car}", car);

                var viewModel = new CarDetailViewModel {
                    VIN = car.VIN,
                    Make = car.Model.Manufacturer.Name,
                    Model = car.Model.Name,
                    Year = car.Year,
                    DriveTrain = car.Model.DriveTrain,
                    Transmission = car.Model.Transmission,
                    Classification = car.Model.BodyConfiguration.Name,
                    DoorCount = car.Model.BodyConfiguration.DoorCount,
                    WindowCount = car.Model.BodyConfiguration.WindowCount,
                    SeatCount = car.Model.BodyConfiguration.SeatCount,
                    ServiceRecords = car.ServiceRecords?.Select(sr => new ServiceRecordViewModel {
                        ServiceDate = sr.ServiceDate.ToLocalTime(),
                        MileageToDate = sr.MileageToDate,
                        Description = sr.Description,
                    }).OrderByDescending(sr => sr.ServiceDate)
                };
                return View(viewModel);
            } catch (InvalidVinException e) {
                _logger.LogError("Attempted to retrieve details for car with invalid vin: {vin}", vin);
                return RedirectToAction("Error", "Home");
            } catch {
                _logger.LogError("Unexpected error while retrieving details for a car with vin: {vin}", vin);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}

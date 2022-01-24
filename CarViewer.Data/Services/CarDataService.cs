using CarViewer.Data.Domain;
using CarViewer.Data.Enums;
using CarViewer.Data.Exceptions;
using CarViewer.Data.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CarViewer.Data.Services {
    public class CarDataService : ICarDataService {
        private readonly CarContext _carContext;
        private readonly IVinService _vinService;

        public CarDataService(CarContext carContext, IVinService vinService) {
            _carContext = carContext;
            _vinService = vinService;
        }

        public IEnumerable<Car> GetAllMakeAndModel() {
            //Eagerly load 
            return _carContext.Cars
                .Include(car => car.Model)
                .ThenInclude(model => model.Manufacturer);
        }

        public IEnumerable<Car> GetAllModelByTransmission(Transmission transmission) {
            return _carContext.Cars
                .Include(car => car.Model)
                .Where(car => car.Model.Transmission == transmission);
        }

        public Car? FindByVIN(string vin) {
            if (!_vinService.ValidateVIN(vin)) {
                throw new InvalidVinException();
            }

            var car = _carContext.Cars
                .Include(_car => _car.Model)
                .Include(_car => _car.Model.Manufacturer)
                .Include(_car => _car.Model.BodyConfiguration)
                .Include(_car => _car.ServiceRecords)
                .FirstOrDefault(x => x.VIN == vin);

            return car;
        }

        public void AddServiceRecord(Car car, int mileageToDate, string description, DateTime serviceDate) {
            if (car == null) {
                throw new ArgumentNullException(nameof(car));
            }

            //Id is not necessary here, as EF will automatically handle and increment the Id itself
            var serviceRecord = new ServiceRecord {
                MileageToDate = mileageToDate,
                Description = description,
                ServiceDate = serviceDate
            };

            if (car.ServiceRecords == null) {
                car.ServiceRecords = new List<ServiceRecord>();
            }

            car.ServiceRecords.Add(serviceRecord);
        }

        public void SeedDemoData() {
            //Assumes that the database is already seeded, early exit
            if (_carContext.Cars.Any()) {
                return;
            }

            _carContext.Manufacturers.AddRange(
                new Manufacturer { Id = 1, Name = "Subaru" },
                new Manufacturer { Id = 2, Name = "Volkswagen" }
            );

            _carContext.BodyConfigurations.Add(
                new BodyConfiguration {
                    Id = 1,
                    Name = "Sedan",
                    DoorCount = 4,
                    WindowCount = 4,
                    SeatCount = 5
                }
            );

            _carContext.Models.AddRange(
                new Model {
                    Id = 1,
                    ManufacturerId = 1,
                    Name = "WRX",
                    DriveTrain = DriveTrain.AllWheel,
                    Transmission = Transmission.Manual,
                    BodyConfigurationId = 1
                },
                new Model {
                    Id = 2,
                    ManufacturerId = 2,
                    Name = "Passat",
                    DriveTrain = DriveTrain.FrontWheel,
                    Transmission = Transmission.Automatic,
                    BodyConfigurationId = 1
                }
            );

            _carContext.Cars.AddRange(
                new Car {
                    VIN = "1BWK5N64AM5PFA5Y",
                    ModelId = 1,
                    Mileage = 150,
                    Year = 2021
                },
                new Car {
                    VIN = "9BWK5N64AM5PFA5V",
                    ModelId = 2,
                    Mileage = 110_000,
                    Year = 2002
                }
            );

            _carContext.SaveChanges();

            foreach (var car in _carContext.Cars) {
                AddServiceRecord(car, car.Mileage - 10, "Standard oil change", DateTime.UtcNow);
                AddServiceRecord(car, car.Mileage - 50, "Standard wheel rotation", DateTime.UtcNow.AddDays(-60));
                AddServiceRecord(car, car.Mileage - 100, "Standard brakepad replacement", DateTime.UtcNow.AddDays(-120));
            }

            _carContext.SaveChanges();
        }
    }
}

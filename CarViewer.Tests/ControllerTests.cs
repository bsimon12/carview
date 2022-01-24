using CarViewer.Controllers;
using CarViewer.Data.Domain;
using CarViewer.Data.Services.Contracts;
using CarViewer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
namespace CarViewer.Tests {
    public class ControllerTests {
        [Test]
        public void HomeController_Index_ReturnsCarViewModels() {
            var mockCarDataService = new Mock<ICarDataService>();
            var mockLogger = new Mock<ILogger<HomeController>>();

            mockCarDataService
                .Setup(m => m.GetAllMakeAndModel())
                .Returns(new List<Car> {
                    new Car {
                        VIN = "1BWK5N64AM5PFA5Y",
                        ModelId = 1,
                        Mileage = 150,
                        Year = 2021,
                        Model = new Model {
                            Id = 1,
                            Name = "WRX",
                            Manufacturer = new Manufacturer {
                                Id = 1,
                                Name = "Subaru"
                            },
                            BodyConfiguration = new BodyConfiguration {
                                Id = 1,
                                Name = "Sedan"
                            }
                        }
                    },
                    new Car {
                        VIN = "9BWK5N64AM5PFA5V",
                        ModelId = 2,
                        Mileage = 110_000,
                        Year = 2002,
                        Model = new Model {
                            Id = 2,
                            Name = "Passat",
                            Manufacturer = new Manufacturer {
                                Id = 2,
                                Name = "Volkswagen"
                            },
                            BodyConfiguration = new BodyConfiguration {
                                Id = 1,
                                Name = "Sedan"
                            }
                        }
                    }
                });


            var homeController = new HomeController(mockLogger.Object, mockCarDataService.Object);
            var result = homeController.Index();

            Assert.IsInstanceOf<ViewResult>(result);

            var model = (result as ViewResult).Model;

            Assert.IsInstanceOf<IEnumerable<CarViewModel>>(model);

            var cars = model as IEnumerable<CarViewModel>;

            Assert.IsNotNull(cars);
            Assert.That(cars?.Count() == 2);
        }

        [Test]
        public void Details_NonExistentVIN_ReturnsNotFound() {
            var mockCarDataService = new Mock<ICarDataService>();
            var mockLogger = new Mock<ILogger<DetailsController>>();
            var controller = new DetailsController(mockLogger.Object, mockCarDataService.Object);
            var result = controller.Details("valid_vin");
        }

        [Test]
        public void DetailsController_ValidVin_DetailsReturnsCarDetailViewModel() {
            var mockCarDataService = new Mock<ICarDataService>();
            var mockLogger = new Mock<ILogger<DetailsController>>();

            var testCar = new Car {
                VIN = "valid_vin",
                Year = 2021,
                Mileage = 150,
                Model = new Model {
                    Id = 1,
                    Manufacturer = new Manufacturer {
                        Id = 1,
                        Name = "Subaru"
                    },
                    Name = "WRX",
                    DriveTrain = Data.Enums.DriveTrain.AllWheel,
                    Transmission = Data.Enums.Transmission.Manual,
                    BodyConfiguration = new BodyConfiguration {
                        Id = 1,
                        Name = "Sedan",
                        WindowCount = 4,
                        DoorCount = 4,
                        SeatCount = 5
                    }
                },
                ServiceRecords = new List<ServiceRecord>()
            };

            mockCarDataService
                .Setup(m => m.FindByVIN(It.IsAny<string>()))
                .Returns(testCar);

            var controller = new DetailsController(mockLogger.Object, mockCarDataService.Object);
            var result = controller.Details("valid_vin");

            Assert.IsInstanceOf<ViewResult>(result);

            var model = (result as ViewResult).Model;

            Assert.IsInstanceOf<CarDetailViewModel>(model);

            var viewModel = model as CarDetailViewModel;

            Assert.IsNotNull(viewModel);
            Assert.That(
                    viewModel.VIN            == testCar.VIN
                &&  viewModel.Make           == testCar.Model.Manufacturer.Name
                &&  viewModel.Model          == testCar.Model.Name
                &&  viewModel.Year           == testCar.Year
                &&  viewModel.DriveTrain     == testCar.Model.DriveTrain
                &&  viewModel.Transmission   == testCar.Model.Transmission
                &&  viewModel.Classification == testCar.Model.BodyConfiguration.Name
                &&  viewModel.DoorCount      == testCar.Model.BodyConfiguration.DoorCount
                &&  viewModel.WindowCount    == testCar.Model.BodyConfiguration.WindowCount
                &&  viewModel.SeatCount      == testCar.Model.BodyConfiguration.SeatCount
                && !viewModel.ServiceRecords.Any()
            );
        }
    }
}

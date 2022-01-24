using CarViewer.Data;
using CarViewer.Data.Domain;
using CarViewer.Data.Enums;
using CarViewer.Data.Services;
using CarViewer.Data.Services.Contracts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Data.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using CarViewer.Data.Exceptions;

namespace CarViewer.Tests {
    /// <summary>
    /// There is a lot of duplication in these tests. However, I generally find that some amount of repeated boilerplate improves readability in tests. 
    /// For example, if a single test is failing and I navigate to the test, I want to see the entire arrangement of the test as quickly as possible without trying to understand the context from the entire suite.
    /// </summary>
    public class DataTests {
        private readonly Mock<IVinService> _mockVinService = new Mock<IVinService>();

        [Test]
        public void GetAllMakeAndModel_ReturnsAllCarsWithMetadata() {
            //Create an in memory SQLite database that will live for the entire test and then be disposed
            using var connection = new SqliteConnection("DataSource=:memory:");
            //EF will never close an existing connection on its own, so the lifetime of this connection is managed manually
            connection.Open();
            using var context = initializeDbContext(connection);

            var carDataService = new CarDataService(context, _mockVinService.Object);
            var allCars = carDataService.GetAllMakeAndModel();

            Assert.AreEqual(allCars.Count(), context.Cars.Count());
        }

        [Test]
        public void GetAllModelByTransmission_ReturnsProperData() {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            using var context = initializeDbContext(connection);
            var carDataService = new CarDataService(context, _mockVinService.Object);


            var manualCars = carDataService.GetAllModelByTransmission(Transmission.Manual);
            var automaticCars = carDataService.GetAllModelByTransmission(Transmission.Automatic);

            Assert.IsTrue(manualCars.Count() == 1);
            Assert.IsTrue(automaticCars.Count() == 1);

            Assert.IsTrue(manualCars.First().Model.Transmission == Transmission.Manual);
            Assert.IsTrue(automaticCars.First().Model.Transmission == Transmission.Automatic);
        }

        [Test]
        public void FindByVIN_InvalidVIN_ThrowsInvalidVinException() {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            using var context = initializeDbContext(connection);

            _mockVinService.Setup(m => m.ValidateVIN(It.IsAny<string>())).Returns(false);
            var carDataService = new CarDataService(context, _mockVinService.Object);

            Assert.Throws<InvalidVinException>(() => carDataService.FindByVIN("invalid_vin"));
        }

        [Test]
        public void FindByVIN_NonExistentVIN_ReturnsNull() {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            using var context = initializeDbContext(connection);
            _mockVinService.Setup(m => m.ValidateVIN(It.IsAny<string>())).Returns(true);
            var carDataService = new CarDataService(context, _mockVinService.Object);

            Assert.IsNull(carDataService.FindByVIN("valid_vin"));
        }

        [Test]
        public void FindByVIN_ExistentVIN_ReturnsCar() {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            using var context = initializeDbContext(connection);
            _mockVinService.Setup(m => m.ValidateVIN(It.IsAny<string>())).Returns(true);
            var carDataService = new CarDataService(context, _mockVinService.Object);
            var car = carDataService.FindByVIN("1BWK5N64AM5PFA5Y");

            Assert.IsNotNull(car);
            Assert.IsTrue(
                car?.Model?.Manufacturer?.Name == "Subaru" 
             && car.Model.Name == "WRX"
             && car.Mileage == 150
             && car.Year == 2021
            );
        }

        [Test]
        public void FindByVIN_HasServiceHistory_IncludesServiceHistory() {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            using var context = initializeDbContext(connection);
            _mockVinService.Setup(m => m.ValidateVIN(It.IsAny<string>())).Returns(true);
            var carDataService = new CarDataService(context, _mockVinService.Object);
            var car = carDataService.FindByVIN("1BWK5N64AM5PFA5Y");

            Assert.IsNotNull(car?.ServiceRecords);
            Assert.IsTrue(car?.ServiceRecords.Count == 3);
        }

        [Test]
        public void FindByVIN_HasNoServiceHistory_ReturnsEmpty() {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            using var context = initializeDbContext(connection);
            _mockVinService.Setup(m => m.ValidateVIN(It.IsAny<string>())).Returns(true);
            var carDataService = new CarDataService(context, _mockVinService.Object);
            var car = carDataService.FindByVIN("9BWK5N64AM5PFA5V");

            Assert.IsEmpty(car.ServiceRecords);
        }

        /// <summary>
        /// Initialize a CarContext over a live DbConnection and seeds it with test data
        /// </summary>
        /// <param name="connection">An open DbConnection which is assumed to be an in-memory SQLite connection</param>
        /// <returns>A CarContext over an in-memory database seeded with test data</returns>
        private CarContext initializeDbContext(DbConnection connection) {
            var dbContextOptions = new DbContextOptionsBuilder<CarContext>()
                .UseSqlite(connection)
                .Options;

            var context = new CarContext(dbContextOptions);

            //Create a fresh databse from latest domain model
            context.Database.EnsureCreated();
            SeedTestDatabase(context);
            return context;
        }

        private void SeedTestDatabase(CarContext context) {
            context.Manufacturers.AddRange(
                new Manufacturer { Id = 1, Name = "Subaru" },
                new Manufacturer { Id = 2, Name = "Volkswagen" }
            );

            context.BodyConfigurations.Add(
                new BodyConfiguration {
                    Id = 1,
                    Name = "Sedan",
                    DoorCount = 4,
                    WindowCount = 4,
                    SeatCount = 5
                }
            );

            context.Models.AddRange(
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

            context.Cars.AddRange(
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

            context.SaveChanges();

            context.Cars.First(c => c.VIN == "1BWK5N64AM5PFA5Y").ServiceRecords = new List<ServiceRecord>() {
                new ServiceRecord {
                    MileageToDate = 100,
                    Description = "Standard oil change",
                    ServiceDate = new DateTime(2022, 1, 1),
                },
                new ServiceRecord {
                    MileageToDate = 200,
                    Description = "Standard oil change",
                    ServiceDate = new DateTime(2022, 3, 1),
                },
                new ServiceRecord {
                    MileageToDate = 1000,
                    Description = "Standard oil change",
                    ServiceDate = new DateTime(2022, 9, 1),
                }
            };

            context.SaveChanges();
        }
    }
}
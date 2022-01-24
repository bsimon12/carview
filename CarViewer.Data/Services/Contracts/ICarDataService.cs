using CarViewer.Data.Domain;
using CarViewer.Data.Enums;
using CarViewer.Data.Exceptions;

namespace CarViewer.Data.Services.Contracts {
    public interface ICarDataService {

        /// <summary>
        /// Retrieves all existing cars with information about the model and manufacturer for display. 
        /// 
        /// Executes a query like so: 
        /// 
        /// <code>
        ///  SELECT "c"."VIN", "c"."Mileage", "c"."ModelId", "c"."Year", "m"."Id", "m"."BodyConfigurationId", "m"."DriveTrain", "m"."ManufacturerId", "m"."Name", "m"."Transmission", "m0"."Id", "m0"."Name"
        ///  FROM "Car" AS "c"
        ///  INNER JOIN "Model" AS "m" ON "c"."ModelId" = "m"."Id"
        ///  INNER JOIN "Manufacturer" AS "m0" ON "m"."ManufacturerId" = "m0"."Id"
        /// </code>
        /// </summary>
        /// <returns>
        /// Returns all cars with make and model information. Model and Manufacturer are guaranteed to not be null.
        /// </returns>
        public IEnumerable<Car> GetAllMakeAndModel();

        /// <summary>
        /// Retrieves a collection of cars with all model information that have the given transmission type 
        /// </summary>
        /// <param name="transmission">
        /// The kind of transmission to filter by (Automatic, Manual)
        /// </param>
        /// <returns>A collection of all cars with model information matching a specific kind of transmission</returns>
        public IEnumerable<Car> GetAllModelByTransmission(Transmission transmission);

        /// <summary>
        /// Finds a car by VIN number.
        /// <br></br>
        /// <br></br>
        /// Executes the following query:
        /// <code>
        /// SELECT "c"."VIN", "c"."Mileage", "c"."ModelId", "c"."Year", "m"."Id", "m"."BodyConfigurationId", "m"."DriveTrain", "m"."ManufacturerId", "m"."Name", "m"."Transmission", "m0"."Id", "m0"."Name", "b"."Id", "b"."DoorCount", "b"."Name", "b"."SeatCount", "b"."WindowCount"
        /// FROM "Car" AS "c"
        /// INNER JOIN "Model" AS "m" ON "c"."ModelId" = "m"."Id"
        /// INNER JOIN "Manufacturer" AS "m0" ON "m"."ManufacturerId" = "m0"."Id"
        /// INNER JOIN "BodyConfiguration" AS "b" ON "m"."BodyConfigurationId" = "b"."Id"
        /// WHERE "c"."VIN" = @__vin_0
        /// LIMIT 1
        /// </code>
        /// </summary>
        /// <param name="vin">A valid VIN number</param>
        /// 
        /// <exception cref="InvalidVinException">
        /// Thrown if the given VIN is invalid.
        /// </exception>
        /// <returns>The matching car or null if no car is found</returns>
        public Car? FindByVIN(string vin);

        /// <summary>
        /// Creates a new service record
        /// </summary>
        /// <param name="car"></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if car is null
        /// </exception>
        public void AddServiceRecord(Car car, int mileageToDate, string description, DateTime serviceDate);


        /// <summary>
        /// Seeds the underlying data store with some example data (for demo purposes);
        /// </summary>
        public void SeedDemoData();
    }
}

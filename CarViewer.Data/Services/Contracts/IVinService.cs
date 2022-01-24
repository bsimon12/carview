namespace CarViewer.Data.Services.Contracts {
    public interface IVinService {
        /// <summary>
        /// Determines if a given VIN is of a valid form. A valid VIN is exactly 16 characters and starts with a decimal and ends with letter
        /// </summary>
        /// <param name="vin">A potentially valid VIN</param>
        /// <returns>true if the given VIN is valid, false if it is invalid</returns>
        public bool ValidateVIN(string vin);
    }
}

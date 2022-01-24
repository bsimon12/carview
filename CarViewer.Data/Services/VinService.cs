using CarViewer.Data.Services.Contracts;

namespace CarViewer.Data.Services {
    public class VinService : IVinService {
        public bool ValidateVIN(string vin) {
            if (string.IsNullOrWhiteSpace(vin)) { 
                return false;
            }

            return vin.Length == 16 && char.IsDigit(vin.First()) && char.IsLetter(vin.Last());
        }
    }
}

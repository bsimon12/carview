using CarViewer.Data.Enums;

namespace CarViewer.Models {
    public class CarDetailViewModel {
        public string? VIN { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public DriveTrain DriveTrain { get; set; } 
        public Transmission Transmission { get; set; }
        public string? Classification { get; set; }
        public int DoorCount { get; set; }
        public int WindowCount { get; set; }
        public int SeatCount { get; set; }

        public IEnumerable<ServiceRecordViewModel>? ServiceRecords { get; set; }
    }
}

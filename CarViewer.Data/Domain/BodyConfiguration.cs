using System.ComponentModel.DataAnnotations;

namespace CarViewer.Data.Domain {
    /// <summary>
    /// Represents a discrete form factor of a car
    /// </summary>
    public class BodyConfiguration {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int DoorCount { get; set; }

        [Required]
        public int WindowCount { get; set; }

        [Required]
        public int SeatCount { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CarViewer.Data.Domain {
    /// <summary>
    /// Represents a concrete vehicle that exists in the real world
    /// </summary>
    public class Car {
        /**
         Constrain the key at a schema level to be at most 16 characters. If this were to grow in the future, a migration would need to be written.
         * 
        **/
        [Key]
        [StringLength(16)]
        public string VIN { get; set; }

        [Required]
        public int ModelId { get; set; }

        public Model? Model { get; set; }

        [Required]
        public int Mileage { get; set; }

        [Required]
        public int Year { get; set; }

        public ICollection<ServiceRecord>? ServiceRecords { get; set; }
    }
}

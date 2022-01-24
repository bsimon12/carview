using CarViewer.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace CarViewer.Data.Domain {
    public class Model {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ManufacturerId { get; set; }

        public Manufacturer? Manufacturer { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DriveTrain DriveTrain { get; set; }

        [Required]
        public Transmission Transmission { get; set; }

        [Required]
        public int BodyConfigurationId { get; set; }

        public BodyConfiguration? BodyConfiguration { get; set; }
    }
}

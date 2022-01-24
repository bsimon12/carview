using System.ComponentModel.DataAnnotations;

namespace CarViewer.Data.Domain {
    public class Manufacturer {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}

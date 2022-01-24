using System.ComponentModel.DataAnnotations;

namespace CarViewer.Data.Domain {
    public class ServiceRecord {

        [Key]
        public int Id { get; set; }

        [Required]
        public int MileageToDate { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime ServiceDate { get; set; }
    }
}

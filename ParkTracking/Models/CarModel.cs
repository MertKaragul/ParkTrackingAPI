using System.ComponentModel.DataAnnotations;

namespace ParkTracking.Models {
	public class CarModel {
        [Key]
        public int CarId { get; set; }
        public int UserID { get; set; }
        public string CarBrand { get; set; }
        public string CarColor { get; set; }
        public string CarPlate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ParkTracking.Models {
	public class ParkModel {
		[Key]
        public int ParkID { get; set; }
		public int UserID { get; set; }
		public int ParkNumber { get; set; }
    }
}

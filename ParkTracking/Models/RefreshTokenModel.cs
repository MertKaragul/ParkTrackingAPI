using System.ComponentModel.DataAnnotations;

namespace ParkTracking.Models {
	public class RefreshTokenModel {
        [Key]
        public int TokenId { get; set; }
        public int UserId { get; set; }
        public string RefreshToken { get; set; }    
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}

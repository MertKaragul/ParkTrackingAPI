using System.ComponentModel.DataAnnotations;

namespace ParkTracking.Models
{
    public class ParkArea
    {
        [Key]
        public int Key { get; set; }
        public int ParkID { get; set; }
        public ParkStatus Status { get; set; }
    }
}

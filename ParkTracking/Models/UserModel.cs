using System.ComponentModel.DataAnnotations;

namespace ParkTracking.Models {
	public class UserModel {

		[Key]
		public int UserID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public string IdentyNumber { get; set; }
        public Roles Roles { get; set; } = Roles.USER;
    }
}

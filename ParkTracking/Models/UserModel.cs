using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ParkTracking.Models {
	public class UserModel {

		[Key]
		public int UserID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public string IdentyNumber { get; set; }
        public Roles Roles { get; set; } = Roles.USER;


        public Claim[] toClaim()
        {
            return new[]
            {
                new Claim(ClaimTypes.Role, Roles.ToString()),
                new Claim("User_id", UserID.ToString()),
            };
        }
    }
}

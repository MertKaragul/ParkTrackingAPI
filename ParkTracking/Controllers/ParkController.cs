using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ParkTracking.Controllers {

	[Route("[controller]")]
	[ApiController]
	public class ParkController : ControllerBase {

		[HttpPost("CarParking")]
		[Authorize(Roles = "ADMIN")]
		public ActionResult CarParking()
		{
			return Ok(new { message = "Working wow! "});
		}


	}
}

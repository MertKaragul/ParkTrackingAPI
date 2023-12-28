using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkTracking.Models;
using ParkTracking.Services.Json_web_token;
using ParkTracking.Services.Managments.ParkManagement;
using ParkTracking.Services.Managments.ParkStatusManagement;
using ParkTracking.Services.Managments.RefreshToken;
using ParkTracking.Services.Managments.UserManagement;

namespace ParkTracking.Controllers {

	[Route("/park/[controller]")]
	[ApiController]
	public class ParkController : ControllerBase {

        private ParkManagment _parkManagement;
        private ParkAreaManagement _parkAreaManagement;
        public ParkController(IConfiguration configuration)
        {
            _parkManagement = new ParkManagment(configuration);
            _parkAreaManagement = new ParkAreaManagement(configuration);

        }

        [HttpPost("/createPark")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> CreatePark(string? identyNumber)
        {
            if (identyNumber == null) return BadRequest(new { message = "Identy number required" });
            var ifUserAlreadyParking = _parkManagement.checkPark(identyNumber);
            if (ifUserAlreadyParking != null) return Ok(new { message = "User already park a car" });
            var findEmptyParkArea = await _parkAreaManagement.findEmptyParkArea();
            if (findEmptyParkArea == null) return NotFound(new { message = "Cannot find empty park area" });
            var parkModel = new ParkModel()
            {
                UserIdenty = identyNumber,
                ParkNumber = findEmptyParkArea.ParkID,
            };
            _parkManagement.createPark(parkModel);

            findEmptyParkArea.Status = ParkStatus.FULL;
            _parkAreaManagement.updateParkStatus(findEmptyParkArea);


            return Ok(findEmptyParkArea);
        }

        [HttpPost("/showParks")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> ShowParks()
        {
            return Ok(_parkAreaManagement.findAllParkAreas());
        }


    }
}

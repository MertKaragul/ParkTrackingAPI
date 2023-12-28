using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using ParkTracking.Models;
using ParkTracking.Services.Database;

namespace ParkTracking.Services.Managments.ParkManagement
{
    public class ParkManagment : IParkManagement
	{
        private readonly IConfiguration _configuration;

        public ParkManagment(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ParkModel? checkPark(string identy)
        {
            using var context = new Context(_configuration);
            return context.Set<ParkModel>().FirstOrDefault(x => x.UserIdenty == identy);
        }

        public void createPark(ParkModel parkModel)
        {
			using var context = new Context(_configuration);
            context.Add<ParkModel>(parkModel);
            context.SaveChanges();
		}

        public void removePark(ParkModel parkModel)
        {
			using var context = new Context(_configuration);
            context.Remove<ParkModel>(parkModel);
            context.SaveChanges();
		}
    }
}

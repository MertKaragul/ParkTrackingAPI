using Microsoft.EntityFrameworkCore;
using ParkTracking.Models;
using ParkTracking.Services.Database;

namespace ParkTracking.Services.Managments.ParkStatusManagement
{
    public class ParkAreaManagement : IParkAreaManagement
    {
        private readonly IConfiguration _configuration;

        public ParkAreaManagement(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<ParkArea> emptyParkAreas()
        {
            using var context = new Context(_configuration);
            return context.Set<ParkArea>().Where(x => x.Status == ParkStatus.EMPTY).ToArray();
        }

        public List<ParkArea> findAllParkAreas()
        {
            using var context = new Context(_configuration);
            return context.Set<ParkArea>().ToList();
        }

        public async Task<ParkArea?> findEmptyParkArea()
        {
            using var context = new Context(_configuration);
            return await context.Set<ParkArea>().FirstOrDefaultAsync(x => x.Status == ParkStatus.EMPTY);
        }

        public async Task<ParkArea?> findParkArea(int parkAreaID)
        {
            using var context = new Context(_configuration);
            return await context.Set<ParkArea>().FirstOrDefaultAsync(x => x.ParkID == parkAreaID);
        }

        public void updateParkStatus(ParkArea parkArea)
        {
            using var context = new Context(_configuration);
            context.Update<ParkArea>(parkArea);
            context.SaveChanges();
        }
    }
}

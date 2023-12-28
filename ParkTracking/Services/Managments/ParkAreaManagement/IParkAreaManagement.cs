using ParkTracking.Models;

namespace ParkTracking.Services.Managments.ParkStatusManagement
{
    public interface IParkAreaManagement
    {
        public void updateParkStatus(ParkArea parkArea);
        public Task<ParkArea?> findEmptyParkArea();
        public List<ParkArea> findAllParkAreas();
    }
}

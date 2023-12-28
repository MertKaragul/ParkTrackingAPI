using ParkTracking.Models;

namespace ParkTracking.Services.Managments.ParkManagement
{
    public interface IParkManagement
    {
        public void createPark(ParkModel parkModel);
        public ParkModel? checkPark(string identy);
        public void removePark(ParkModel parkModel);
    }
}

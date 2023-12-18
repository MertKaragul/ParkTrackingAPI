using ParkTracking.Models;

namespace ParkTracking.Services.Managments.UserManagement
{
    public interface IUserManagement
    {
        void insert(UserModel userModel);
        void update(UserModel userModel);
        void delete(UserModel userModel);
        Task<UserModel?> findUserByModel(UserModel userModel);
        Task<UserModel?> findUserByIdentyNumber(string identyNumber);

        Task<UserModel?> findUserById(int id);
    }
}

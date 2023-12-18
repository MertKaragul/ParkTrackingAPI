using Microsoft.EntityFrameworkCore;
using ParkTracking.Models;
using ParkTracking.Services.Database;

namespace ParkTracking.Services.Managments.UserManagement
{
    public class UserManagement : IUserManagement
    {

        private readonly IConfiguration _configuration;

        public UserManagement(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void delete(UserModel userModel)
        {
            using var context = new Context(_configuration);
            context.Remove(userModel);
            context.SaveChanges();
        }

        public async Task<UserModel?> findUserById(int id)
        {
            using var context = new Context(_configuration);
            UserModel? findUser = await context.Set<UserModel>().FirstOrDefaultAsync(x => x.UserID == id);
            return findUser;
        }

        public async Task<UserModel?> findUserByModel(UserModel userModel)
        {
            using var context = new Context(_configuration);
            UserModel? findUser = await context.Set<UserModel>().FirstOrDefaultAsync(x => x == userModel);
            return findUser;
        }

        public async Task<UserModel?> findUserByIdentyNumber(string identyNumber)
        {
            using var context = new Context(_configuration);
            UserModel? findUser = await context.Set<UserModel>().FirstOrDefaultAsync(x => x.IdentyNumber == identyNumber);
            return findUser;
        }

        public void insert(UserModel userModel)
        {
            using var context = new Context(_configuration);
            context.Add(userModel);
            context.SaveChanges();
        }

        public void update(UserModel userModel)
        {
            using var context = new Context(_configuration);
            context.Update(userModel);
            context.SaveChanges();
        }
    }
}

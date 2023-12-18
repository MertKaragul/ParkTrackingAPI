using Microsoft.EntityFrameworkCore;
using ParkTracking.Models;

namespace ParkTracking.Services.Database {
	public class Context : DbContext {

		private readonly IConfiguration _configuration;

		public Context(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:MSSQLConnection"]);
			base.OnConfiguring(optionsBuilder);
		}

		public DbSet<UserModel> Users { get; set; }
		public DbSet<ParkModel> Park { get; set; }
		public DbSet<CarModel> Car { get; set; }
		public DbSet<RefreshTokenModel> refreshTokens { get; set; }
	}
}
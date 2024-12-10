using Azure.Core.Pipeline;
using game_service.models;
using Microsoft.EntityFrameworkCore;

namespace game_service.context
{
	public class GameDatabaseContext : DbContext
	{
		public GameDatabaseContext(DbContextOptions options) : base(options) { }

		public DbSet<GameSession> GameSessions { get; set; }
		public DbSet<GameAction> GameActions { get; set; }
		public DbSet<GameHistory> GameHistory { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}

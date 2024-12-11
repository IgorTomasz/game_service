using Azure.Core.Pipeline;
using game_service.models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
			modelBuilder.Entity<GameAction>().Property(e => e.ActionData).HasColumnType("nvarchar(max)").HasConversion(
				c => JsonSerializer.Serialize(c, (JsonSerializerOptions)null),
				c => JsonSerializer.Deserialize<Dictionary<string, object>>(c, (JsonSerializerOptions)null));

			modelBuilder.Entity<GameSession>().Property(e => e.SerializedGame).HasColumnType("nvarchar(max)");
		}
	}
}

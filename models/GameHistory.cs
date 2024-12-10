using game_service.classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace game_service.models
{
	public class GameHistory
	{
		[Key]
		public Guid GameHistoryId { get; set; }
		[Required]
		public Guid GameSessionId { get; set; }
		[Required]
		public Guid UserId { get; set; }
		[Required]
		public GameType GameType { get; set; }
		[Required]
		public decimal BetAmount { get; set; }
		[Required]
		public decimal MaxMultiplier { get; set; }
		[Required]
		public decimal Result { get; set; }
		[Required]
		public DateTime Timestamp { get; set; }
		[ForeignKey(nameof(GameSessionId))]
		public virtual GameSession GameSession { get; set; }
	}
}

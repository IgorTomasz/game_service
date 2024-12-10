using game_service.classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace game_service.models
{
	public class GameSession
	{
		[Key]
		public Guid GameSessionId { get; set; }
		[Required]
		public Guid GameId { get; set; }
		[Required]
		public Guid UserId { get; set; }
		[Required] 
		public Guid UserSessionId { get; set; }
		public Guid? GameHistoryId { get; set; }
		[Required]
		public decimal BetAmount { get; set; }
		[Required]
		public GameType GameType { get; set; }
		public decimal? Result {  get; set; }
		[Required]
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		[Required]
		public GameStatus Status { get; set; }
		[Required]
		public decimal CurrentMultiplier { get; set; }
		[Required]
		public bool CashedOutEarly { get; set; }
		public virtual ICollection<GameAction> GameActions { get; set; }
		[ForeignKey(nameof(GameHistoryId))]
		public virtual GameHistory History { get; set; }
	}
}

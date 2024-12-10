using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace game_service.models
{
	public class GameAction
	{
		[Key]
		public Guid GameActionId { get; set; }
		[Required]
		public Guid GameSessionId { get; set; }
		[Required]
		public DateTime Timestamp { get; set; }
		[Required]
		public string Type { get; set; }
		[Required]
		public string ActionData { get; set; }
		[ForeignKey(nameof(GameSessionId))]
		public virtual GameSession GameSession { get; set; }
	}
}

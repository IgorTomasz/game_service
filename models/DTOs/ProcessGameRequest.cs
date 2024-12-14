using game_service.classes;
using System.ComponentModel.DataAnnotations;

namespace game_service.models.DTOs
{
	public class ProcessGameRequest
	{
		[Required]
		public GameType Type { get; set; }
		[Required]
		public Guid UserId { get; set; }
		[Required]
		public Guid UserSessionId { get; set; }
		public Guid? GameSessionId { get; set; }
		[Required] 
		public ActionType Action { get; set; }
		[Required]
		public decimal BetAmount { get; set; }
		[Required]
		public Dictionary<string, object> Data { get; set; }
	}
}

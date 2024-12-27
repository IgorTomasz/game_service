using game_service.classes;

namespace game_service.models.DTOs
{
	public class GameActionResponse
	{
		public GameStatus Status { get; set; }
		public decimal Multiplier { get; set; }
		public decimal Result { get; set; }
		public Dictionary<string, object>? Data { get; set; }
	}
}

using System.ComponentModel.DataAnnotations;

namespace game_service.models
{
	public class Games
	{
		[Key]
		public int GameId { get; set; }
		[Required]
		public string GameName { get; set; } = string.Empty;
		[Required]
		public GameCategory GameCategory {  get; set; }
		[Required]
		public string GameDescription { get; set; } = string.Empty;
		[Required]
		public Dictionary<string, string> GameAdditionalFields {  get; set; }
		[Required]
		public bool IsActive { get; set; }
	}

	public enum GameCategory
	{
		Card, Arcade, Random, Strategy
	}
}

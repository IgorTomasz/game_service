using System.ComponentModel.DataAnnotations;

namespace game_service.models.DTOs
{
	public class CreateGameRequest
	{
		[Required]
		public string GameName { get; set; }
		[Required] 
		public GameCategory GameCategory { get; set; }
		[Required]
		public string GameDescription { get; set; }
		[Required]
		public Dictionary<string, string> GameAdditionalFields { get; set; }
		[Required]
		public bool IsActive { get; set; }
	}
}

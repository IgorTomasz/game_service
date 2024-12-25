using game_service.classes;
using System.ComponentModel.DataAnnotations;

namespace game_service.models.DTOs
{
	public class UserSessionRequest
	{
		[Required]
		public Guid UserId { get; set; }
		[Required]
		public Guid UserSessionId { get; set; }
		[Required]
		public GameType GameType { get; set; }
	}
}

namespace game_service.models.DTOs
{
	public class GamesResponse
	{
		public bool Success { get; set; }
		public string Error { get; set; }
		public List<Games> Games { get; set; }
	}
}

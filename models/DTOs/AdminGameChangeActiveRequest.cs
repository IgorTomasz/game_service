namespace game_service.models.DTOs
{
	public class AdminGameChangeActiveRequest
	{
		public List<GameActive> gameActives {  get; set; }
	}

	public class GameActive
	{
		public int GameId { get; set; }
		public bool IsActive { get; set; }
	}
}

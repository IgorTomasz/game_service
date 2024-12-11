namespace game_service.classes.games
{
	public class GameData
	{
		public GameType GameType { get; set; }
		public decimal BetAmount { get; set; }
		public decimal CurrentMultiplier { get; set; }
		public Guid GameId { get; set; }
		public GameStatus Status { get; set; }
		public Dictionary<string, object> GamesValues { get; set; }
	}
}

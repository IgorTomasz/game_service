using game_service.classes.games;

namespace game_service.classes
{
	public interface AbstractGame
	{
		public decimal BetAmount { get; set; }
		public decimal CurrentMultiplier { get; set; }
		public Guid GameId { get; set; }
		public GameStatus Status { get; set; }
		public GameType Type { get; set; }

		public decimal GetMultiplier();
		public decimal GetResult();
		public GameStatus GetStatus();
		public static abstract AbstractGame RestoreGameData(GameData gameData);
		public static abstract AbstractGame CreateGame(decimal betAmount);

	}

	public enum GameType
	{
		Mines, Plinko, Chicken, Dice, BlackJack
	}

	public enum GameStatus
	{
		InProgress, EndedWin, EndedLose
	}
}

using game_service.classes.games;

namespace game_service.classes
{
	public interface AbstractGame
	{
		public decimal BetAmount { get; set; }
		public decimal CurrentMultiplier { get; set; }
		public GameStatus Status { get; set; }
		public GameType Type { get; set; }
		public bool CashOutEarly { get; set; }

		public decimal GetMultiplier();
		public decimal GetCashWon();
		public GameStatus GetStatus();
		public static abstract AbstractGame RestoreGameData(GameData gameData);
		public static abstract AbstractGame CreateGame(decimal betAmount);
		public void InicializeGame(Dictionary<string, object> gameSettings);

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

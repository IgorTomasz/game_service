namespace game_service.classes
{
	public interface AbstractGame
	{
		public decimal BetAmount { get; set; }
		public decimal CurrentMultiplier { get; set; }
		public Guid GameId { get; set; }
		public GameStatus Status { get; set; }

		public decimal GetMultiplier();
		public decimal GetResult();
		public GameStatus GetStatus();
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

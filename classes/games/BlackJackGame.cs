

namespace game_service.classes.games
{
	public class BlackJackGame : AbstractGame
	{
		public decimal BetAmount { get; set; }
		public decimal CurrentMultiplier { get; set; }
		public GameStatus Status { get; set; }
		public Guid GameId { get; set; }
		public GameType Type { get; set; }
		public bool CashOutEarly { get; set; }

		public decimal GetMultiplier()
		{
			return CurrentMultiplier;
		}

		public GameStatus GetStatus()
		{
			return Status;
		}

		public static AbstractGame RestoreGameData(GameData gameData)
		{
			return new BlackJackGame
			{
				GameId = gameData.GameId,
				Status = gameData.Status,
				Type = gameData.GameType,
				BetAmount = gameData.BetAmount
			};
		}

		public static AbstractGame CreateGame(decimal betAmount)
		{
			return new BlackJackGame
			{
				GameId = Guid.NewGuid(),
				Status = GameStatus.InProgress,
				BetAmount = betAmount,
				Type = GameType.BlackJack
			};
		}

		public decimal GetCashWon()
		{
			throw new NotImplementedException();
		}

		public void InicializeGame(Dictionary<string, object> gameSettings)
		{
			throw new NotImplementedException();
		}
	}
}

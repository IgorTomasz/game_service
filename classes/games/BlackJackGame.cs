

namespace game_service.classes.games
{
	public class BlackJackGame : AbstractGame
	{
		public decimal BetAmount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public decimal CurrentMultiplier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid GameId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public GameStatus Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public GameType Type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CashOutEarly { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

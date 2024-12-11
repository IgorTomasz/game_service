
namespace game_service.classes.games
{
	public class PlinkoGame : AbstractGame
	{
		public decimal BetAmount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public decimal CurrentMultiplier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid GameId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public GameStatus Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public GameType Type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


		public decimal GetMultiplier()
		{
			throw new NotImplementedException();
		}

		public decimal GetResult()
		{
			throw new NotImplementedException();
		}

		public GameStatus GetStatus()
		{
			throw new NotImplementedException();
		}

		public static AbstractGame RestoreGameData(GameData gameData)
		{
			return new PlinkoGame
			{
				BetAmount = gameData.BetAmount,
				CurrentMultiplier = gameData.CurrentMultiplier,
				GameId = gameData.GameId,
				Status = gameData.Status,
				Type = gameData.GameType,
			};
		}

		public static AbstractGame CreateGame(decimal betAmount)
		{
			return new PlinkoGame
			{
				BetAmount = betAmount,
				Status = GameStatus.InProgress,
				GameId = Guid.NewGuid(),
				Type = GameType.Plinko,
				CurrentMultiplier = 0
			};
		}
	}
}

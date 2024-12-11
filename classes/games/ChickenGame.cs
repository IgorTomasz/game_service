
namespace game_service.classes.games
{
	public class ChickenGame : AbstractGame
	{
		public decimal BetAmount { get; set; }
		public decimal CurrentMultiplier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid GameId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public GameStatus Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public GameType Type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public static AbstractGame CreateGame(decimal betAmount)
		{
			return new ChickenGame
			{
				BetAmount = betAmount,
				Status = GameStatus.InProgress,
				GameId = Guid.NewGuid(),
				Type = GameType.Chicken,
				CurrentMultiplier = 0
			};
		}

		public static AbstractGame RestoreGameData(GameData gameData)
		{
			return new ChickenGame
			{
				BetAmount = gameData.BetAmount,
				Status =gameData.Status,
				GameId =gameData.GameId,
				Type = gameData.GameType,
				CurrentMultiplier = gameData.CurrentMultiplier,
			};
		}

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
	}
}

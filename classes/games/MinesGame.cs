
namespace game_service.classes.games
{
	public class MinesGame : AbstractGame
	{



		public decimal BetAmount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public decimal CurrentMultiplier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid GameId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public GameStatus Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public GameType Type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public int[] field = new int[25];

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
			return new MinesGame
			{
				BetAmount = gameData.BetAmount,
				CurrentMultiplier = gameData.CurrentMultiplier,
				GameId = gameData.GameId,
				Status = gameData.Status,
				Type = gameData.GameType,
				field = (int[])gameData.GamesValues["field"]
			};
		}

		public static AbstractGame CreateGame(decimal betAmount)
		{
			return new MinesGame
			{
				BetAmount = betAmount,
				GameId = Guid.NewGuid(),
				Status = GameStatus.InProgress,
				Type = GameType.Mines,
				CurrentMultiplier = 0
			};

	}
	}
}

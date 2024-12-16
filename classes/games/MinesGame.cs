

using game_service.models.DTOs;

namespace game_service.classes.games
{
	public class MinesGame : AbstractGame
	{



		public decimal BetAmount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public decimal CurrentMultiplier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid GameId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public GameStatus Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public GameType Type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CashOutEarly { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public int[] field = new int[25];
		public int MinesCount { get; set; }
		public int DiscoveredDiamonds { get; set; }

		public decimal GetMultiplier()
		{
			return CurrentMultiplier;
		}

		public decimal GetCashWon()
		{
			return BetAmount * CurrentMultiplier;
		}

		public GameStatus GetStatus()
		{
			return Status;
		}

		public decimal CalculateMultiplier()
		{
			var diamonds = 25-MinesCount;
			var multi = (diamonds / MinesCount) + 1 + (((DiscoveredDiamonds / diamonds) * diamonds) / 2) * 0.98;
			return decimal.Parse(multi.ToString());
		}

		public bool IsGameOver(MinesPosition position)
		{
			var index = position.Y * 5 + position.X;
			if(field[index] == 0)
			{
				return true;
			}
			DiscoveredDiamonds++;
			return false;
		}

		public void PlaceMines(int minesCount)
		{

		}

		public void ValidateMove(MinesPosition position)
		{
			var isGameOver = IsGameOver(position);
			if (!isGameOver)
			{
				CurrentMultiplier = CalculateMultiplier();
			}

			Status = GameStatus.EndedLose;
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

		public void InicializeGame(Dictionary<string, object> gameSettings)
		{
			MinesCount = (int)gameSettings["Mines"];
			DiscoveredDiamonds = 0;
			PlaceMines(MinesCount);
		}
	}
}

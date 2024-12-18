

using game_service.models.DTOs;
using System.Text.Json;

namespace game_service.classes.games
{
	public class MinesGame : AbstractGame
	{



		public decimal BetAmount { get; set; }
		public decimal CurrentMultiplier { get; set; }
		public Guid GameId { get; set; }
		public GameStatus Status { get; set; }
		public GameType Type { get; set; }
		public bool CashOutEarly { get; set; }

		public int[] Field = new int[25];
		public int MinesCount { get; set; }
		public int DiscoveredDiamonds { get; set; }
		public int _fieldCount = 25;

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
			var multi = (((diamonds / MinesCount) + 1) + (((DiscoveredDiamonds / diamonds) * diamonds) / 2)) * 0.98;
			return decimal.Parse(multi.ToString());
		}

		public bool IsGameOver(MinesPosition position)
		{
			var index = position.Y-1 * 5 + position.X;
			if(Field[index] == 0)
			{
				return true;
			}
			DiscoveredDiamonds++;
			return false;
		}

		public void PlaceMines(int minesCount)
		{
			for (int i = 0; i < minesCount; i++)
			{
				Field[i] = 0;
			}
			for(int i = minesCount;i < _fieldCount; i++)
			{
				Field[i] = 1;
			}

			Random.Shared.Shuffle(Field);
			Console.WriteLine(Field);
		}

		public bool ValidateMove(MinesPosition position)
		{
			var isGameOver = IsGameOver(position);
			if (!isGameOver)
			{
				CurrentMultiplier = CalculateMultiplier();
				if(DiscoveredDiamonds == _fieldCount - MinesCount)
				{
					Status = GameStatus.EndedWin;
					return true;
				}
				return false;
			}

			Status = GameStatus.EndedLose;
			return true;
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
				Field = JsonSerializer.Deserialize<int[]>(gameData.GamesValues["Field"].ToString()),
				DiscoveredDiamonds =JsonSerializer.Deserialize<int>(gameData.GamesValues["DiscoveredDiamonds"].ToString()),
				MinesCount = JsonSerializer.Deserialize<int>(gameData.GamesValues["MinesCount"].ToString())
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
			MinesCount = JsonSerializer.Deserialize<int>(gameSettings["MinesCount"].ToString());
			PlaceMines(MinesCount);
		}
	}
}

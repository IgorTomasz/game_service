

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
			decimal decimalDiamonds = Decimal.Parse(diamonds.ToString());
			decimal decimalDiscovered = Decimal.Parse(DiscoveredDiamonds.ToString());
			decimal baseMulti = (MinesCount / decimalDiamonds) + 1.0m;
			decimal discoveredPercentage = decimalDiscovered / decimalDiamonds;
			decimal progressBoost = (discoveredPercentage * decimalDiscovered)/2.0m;
			decimal multi = baseMulti + progressBoost;
			decimal ret = multi * 0.98m;
			return ret;
		}

		public bool IsGameOver(MinesPosition position)
		{
			var index = (position.Y-1) * 5 + position.X;
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
			CurrentMultiplier = CalculateMultiplier();
			var isGameOver = IsGameOver(position);
			if (!isGameOver)
			{
				if(DiscoveredDiamonds == _fieldCount - MinesCount)
				{
					CurrentMultiplier = CalculateMultiplier();
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
			var disc = JsonSerializer.Deserialize<int>(gameData.GamesValues["DiscoveredDiamonds"].ToString());
			var field = JsonSerializer.Deserialize<int[]>(gameData.GamesValues["Field"].ToString());
			var mines = JsonSerializer.Deserialize<int>(gameData.GamesValues["MinesCount"].ToString());
			MinesGame game = new MinesGame
			{
				BetAmount = gameData.BetAmount,
				CurrentMultiplier = gameData.CurrentMultiplier,
				GameId = gameData.GameId,
				Status = gameData.Status,
				Type = gameData.GameType,
				Field = field,
				DiscoveredDiamonds = disc,
				MinesCount = mines
			};
			return game;
		}

		public static AbstractGame CreateGame(decimal betAmount)
		{
			Guid guid = Guid.NewGuid();
			return new MinesGame
			{
				BetAmount = betAmount,
				GameId = guid,
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

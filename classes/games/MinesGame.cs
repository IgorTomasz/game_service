

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

		private int[] Field = new int[25];
		private int MinesCount { get; set; }
		private int DiscoveredDiamonds { get; set; }
		private int _fieldCount = 25;

		public decimal GetMultiplier()
		{
			return CurrentMultiplier;
		}

		public GameStatus GetStatus()
		{
			return Status;
		}

		public int[] GetField()
		{
			return Field;
		}

		public int GetMinesCount()
		{
			return MinesCount;
		}

		public int GetDiscoveredDiamonds()
		{
			return DiscoveredDiamonds;
		}

		private decimal CalculateMultiplier()
		{
			var diamonds = _fieldCount-MinesCount;
			decimal decimalDiamonds = Decimal.Parse(diamonds.ToString());
			decimal decimalDiscovered = Decimal.Parse(DiscoveredDiamonds.ToString());
			decimal decimalMinesCount = Decimal.Parse(MinesCount.ToString());
			decimal baseMulti = (decimalMinesCount / decimalDiamonds) + 1.0m;
			decimal discoveredPercentage = decimalDiscovered / decimalDiamonds;
			decimal progressBoost = (discoveredPercentage * decimalDiscovered)/2.0m;
			decimal multi = baseMulti + progressBoost;
			decimal ret = multi * 0.98m;
			return ret;
		}

		private bool IsGameOver(MinesPosition position)
		{
			var index = position.Y * 5 + position.X;
			if(Field[index] == 0)
			{
				return true;
			}
			DiscoveredDiamonds++;
			return false;
		}

		private void PlaceMines(int minesCount)
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
			MinesGame game = new MinesGame
			{
				BetAmount = betAmount,
				GameId = guid,
				Status = GameStatus.InProgress,
				Type = GameType.Mines,
				CurrentMultiplier = 0
			};
			return game;

	}

		public void InicializeGame(Dictionary<string, object> gameSettings)
		{
			MinesCount = JsonSerializer.Deserialize<int>(gameSettings["MinesCount"].ToString());
			PlaceMines(MinesCount);
		}

		public void CashOut()
		{
			Status = GameStatus.EndedWin;
			CashOutEarly = true;
		}

		public Guid GetGameId()
		{
			return GameId;
		}

		public decimal GetWinnedAmount()
		{
			return CurrentMultiplier * BetAmount;
		}
	}
}



using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Security.Cryptography;
using System.Text.Json;

namespace game_service.classes.games
{
	public enum Difficulty
	{
		Easy, Medium, Hard
	}

	public class PlinkoGame : AbstractGame
	{
		public decimal BetAmount { get; set; }
		public decimal CurrentMultiplier { get; set; }
		public GameStatus Status { get; set; }
		public Guid GameId { get; set; }
		public GameType Type { get; set; }
		public bool CashOutEarly { get; set; }
		private int RowsCount { get; set; }
		private Difficulty ChoosenDifficulty { get; set; }
		private double FinalBallPosition { get; set; }
		private Dictionary<string, double[]> PlinkoPositions { get; set; }
		private decimal[] MultipliersEasy = { 16, 9, 2, 1.4m, 1.4m,1.2m, 1.1m, 1, 0.5m, 1, 1.1m, 1.2m, 1.4m, 1.4m, 2, 9, 16 };
		private decimal[] MultipliersMedium = { 110, 41, 10, 5, 3, 1.5m, 1m, 0.5m, 0.3m, 0.5m, 1, 1.5m, 3, 5, 10, 41, 110 };
		private decimal[] MultipliersHard = { 1000, 130, 26, 9, 4, 2, 0.2m, 0.2m, 0.2m, 0.2m, 0.2m, 2, 4, 9, 26, 130, 1000 };
		private char[] Path { get; set; }

		public decimal GetMultiplier()
		{
			return CurrentMultiplier;
		}

		public GameStatus GetStatus()
		{
			return Status;
		}

		public char[] GetPath()
		{
			return Path;
		}

		public double GetBallPosition()
		{
			return FinalBallPosition;
		}

		public Dictionary<string, double[]> GetPlinkoPositions()
		{
			return PlinkoPositions;
		}

		public Difficulty GetChoosenDifficulty()
		{
			return ChoosenDifficulty;
		}

		public int GetRowsCount()
		{
			return RowsCount;
		}

		public static AbstractGame RestoreGameData(GameData gameData)
		{
			var ballPos = JsonSerializer.Deserialize<double>(gameData.GamesValues["FinalBallPosition"].ToString());
			var path = JsonSerializer.Deserialize<char[]>(gameData.GamesValues["Path"].ToString());
			
			var rows = JsonSerializer.Deserialize<int>(gameData.GamesValues["Rows"].ToString());
			var diff = JsonSerializer.Deserialize<Difficulty>(gameData.GamesValues["Difficulty"].ToString());
			var file = File.ReadAllText("./classes/games/plinko_positions.json");
			var plinkoPos = JsonSerializer.Deserialize<Dictionary<string, double[]>>(file);
			return new PlinkoGame
			{
				BetAmount = gameData.BetAmount,
				CurrentMultiplier = gameData.CurrentMultiplier,
				Status = gameData.Status,
				Type = gameData.GameType,
				FinalBallPosition = ballPos,
				Path = path,
				ChoosenDifficulty = diff,
				RowsCount = rows,
				PlinkoPositions = plinkoPos
			};
		}

		public static AbstractGame CreateGame(decimal betAmount)
		{
			Guid guid = Guid.NewGuid();
			PlinkoGame game = new PlinkoGame
			{
				BetAmount = betAmount,
				Status = GameStatus.InProgress,
				Type = GameType.Plinko,
				GameId = guid,
				CurrentMultiplier = 0
			};
			return game;
		}

		public decimal GetCashWon()
		{
			return CurrentMultiplier * BetAmount;
		}

		private char[] CalculatePath()
		{
			var rng = RandomNumberGenerator.Create();
			
			for (int i = 0; i < RowsCount; i++)
			{
				byte[] bytes = new byte[4];
				rng.GetBytes(bytes);

				var number = BitConverter.ToInt32(bytes, 0);

				if(number < 0.5)
				{
					Path[i] = 'L';
				}
				else
				{
					Path[i] = 'R';
				}
			}

			return Path;
		}

		private int CalculateMultiplier(char[] path)
		{
			decimal[] temp = new decimal[4];

			switch (ChoosenDifficulty)
			{
				case Difficulty.Easy: temp = MultipliersEasy; break;
				case Difficulty.Medium: temp = MultipliersMedium; break;
				case Difficulty.Hard: temp = MultipliersHard; break;
			}

			var index = temp.Length/2;
			foreach(Char i in Path)
			{
				if(i == 'R') index++;
				if (i == 'L') index--;
			}

			CurrentMultiplier = temp[index];
			return index;
		}

		private double GetPosition(int index)
		{
			double[] positions = PlinkoPositions[index.ToString()];
			var rand = new Random();
			var indexPos = rand.Next(positions.Length);
			return positions[indexPos];
		}

		//trzeba bedzie zmienic typ zwracany
		public void CalculateDrop()
		{
			var path = CalculatePath();
			var multiplier = CalculateMultiplier(path);
			var position = GetPosition(multiplier);
			FinalBallPosition = position;
			
		}

		public void InicializeGame(Dictionary<string, object> gameSettings)
		{
			RowsCount = JsonSerializer.Deserialize<int>(gameSettings["Rows"].ToString());
			ChoosenDifficulty = JsonSerializer.Deserialize<Difficulty>(gameSettings["Difficulty"].ToString());
			Path = new char[RowsCount];
		}

		public void CashOut()
		{
			throw new NotImplementedException();
		}

		public Guid GetGameId()
		{
			return GameId;
		}
	}
}

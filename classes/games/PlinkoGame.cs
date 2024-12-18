

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
		public decimal BetAmount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public decimal CurrentMultiplier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public GameStatus Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public GameType Type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CashOutEarly { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int RowsCount { get; set; }
		public Difficulty ChoosenDifficulty { get; set; }
		public double FinalBallPosition { get; set; }
		public PlinkoPositions PlinkoPositions { get; set; }
		public decimal[] MultipliersEasy = { 16, 9, 2, 1.4m, 1.4m,1.2m, 1.1m, 1, 0.5m, 1, 1.1m, 1.2m, 1.4m, 1.4m, 2, 9, 16 };
		public decimal[] MultipliersMedium = { 110, 41, 10, 5, 3, 1.5m, 1m, 0.5m, 0.3m, 0.5m, 1, 1.5m, 3, 5, 10, 41, 110 };
		public decimal[] MultipliersHard = { 1000, 130, 26, 9, 4, 2, 0.2m, 0.2m, 0.2m, 0.2m, 0.2m, 2, 4, 9, 26, 130, 1000 };
		public char[] Path { get; set; }

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
			return new PlinkoGame
			{
				BetAmount = gameData.BetAmount,
				CurrentMultiplier = gameData.CurrentMultiplier,
				Status = gameData.Status,
				Type = gameData.GameType,
				FinalBallPosition = (double)gameData.GamesValues["FinalBallPosition"],
				Path = (char[])gameData.GamesValues["Path"]
			};
		}

		public static AbstractGame CreateGame(decimal betAmount)
		{
			return new PlinkoGame
			{
				BetAmount = betAmount,
				Status = GameStatus.InProgress,
				Type = GameType.Plinko,
				CurrentMultiplier = 0
			};
		}

		public decimal GetCashWon()
		{
			return CurrentMultiplier * BetAmount;
		}

		public char[] CalculatePath()
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

		public decimal CalculateMultiplier(char[] path)
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

			return temp[index];
		}

		public double GetPosition(decimal multiplier)
		{
			double[] positions = PlinkoPositions.keyValuePairs[multiplier.ToString()];
			var rand = new Random();
			var index = rand.Next(positions.Length);
			return positions[index];
		}

		//trzeba bedzie zmienic typ zwracany
		public void CalculateDrop()
		{
			var path = CalculatePath();
			var multiplier = CalculateMultiplier(path);
			var position = GetPosition(multiplier);
			FinalBallPosition = position;
			CurrentMultiplier = multiplier;
		}

		public void InicializeGame(Dictionary<string, object> gameSettings)
		{
			RowsCount = (int)gameSettings["Rows"];
			ChoosenDifficulty = (Difficulty)gameSettings["Difficulty"];
			PlinkoPositions = JsonSerializer.Deserialize<PlinkoPositions>(File.ReadAllText("./plinko_positions.json"));
		}
	}
}



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
		private readonly decimal[] MultipliersEasy = { 16, 9, 2, 1.4m, 1.4m,1.2m, 1.1m, 1, 0.5m, 1, 1.1m, 1.2m, 1.4m, 1.4m, 2, 9, 16 };
		private readonly decimal[] MultipliersMedium = { 110, 41, 10, 5, 3, 1.5m, 1m, 0.5m, 0.3m, 0.5m, 1, 1.5m, 3, 5, 10, 41, 110 };
		private readonly decimal[] MultipliersHard = { 1000, 130, 26, 9, 4, 2, 0.2m, 0.2m, 0.2m, 0.2m, 0.2m, 2, 4, 9, 26, 130, 1000 };

		private static readonly int[] WeightsMedium = { 1, 2, 8, 15, 25, 35, 475, 475, 475, 475, 475, 35, 25, 15, 8, 2, 1 };
		private static readonly int[] WeightsHard = { 1, 1, 2, 8, 15, 30, 700, 700, 700, 700, 700, 30, 15, 8, 2, 1, 1 };
		private static readonly int[] WeightsEasy = { 1, 2, 8, 15, 15, 20, 470, 470, 470, 470, 470, 20, 15, 15, 8, 2, 1 };


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


		private (decimal[] multipliers, int[] weights) GetMultAndWeights(int rows, Difficulty difficulty)
		{
			var (baseMult, baseWeight) = difficulty switch
			{
				Difficulty.Easy => (MultipliersEasy, WeightsEasy),
				Difficulty.Medium => (MultipliersMedium, WeightsMedium),
				Difficulty.Hard => (MultipliersHard, WeightsHard),
			};

			if (rows == 16)
			{
				return (baseMult, baseWeight);
			}

			var lessRows = rows+1;
			var respMult = new decimal[lessRows];
			var respWeight = new int[lessRows];

			for (int i = 0; i < lessRows; i++)
			{
				var pos = (double)i/(lessRows-1)*(baseMult.Length-1);
				var lowIndex = (int)Math.Floor(pos);
				var highIndex = (int)Math.Ceiling(pos);
				var weight = pos - Math.Floor(pos);

				if(lowIndex == highIndex)
				{
					respMult[i] = baseMult[lowIndex];
					respWeight[i] = baseWeight[lowIndex];
				}
				else
				{
					respMult[i] = baseMult[lowIndex] + (decimal)weight * (baseMult[highIndex] - baseMult[lowIndex]);
					respWeight[i] = (int)(baseWeight[lowIndex] + weight * (baseWeight[highIndex] - baseWeight[lowIndex]));
				}

				
			}

			for (int j = 0; j < respMult.Length; j++)
			{
				respMult[j] = Math.Round(respMult[j], 2);
			}

			return (respMult, respWeight);
		}

		private int GetIndex(int[] weights)
		{
			var total = weights.Sum();
			var rand = new Random();
			var num = rand.Next(total);
			var sum = 0;

			for (int i = 0; i < weights.Length; i++)
			{
				sum += weights[i];
				if(num < sum)
				{
					return i;
				}
			}

			return weights.Length - 1;

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
			var (multipliers, weights) = GetMultAndWeights(RowsCount, ChoosenDifficulty);
			var index = GetIndex(weights);
			var multiplier = multipliers[index];
			CurrentMultiplier = multiplier;
			var position = GetPosition(index);
			FinalBallPosition = position;
			Status = GameStatus.EndedWin;
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

		public decimal GetWinnedAmount()
		{
			return CurrentMultiplier*BetAmount;
		}
	}
}



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
		//private Dictionary<string, double[]> PlinkoPositions { get; set; }
		private Dictionary<string, Dictionary<string, double[]>> PlinkoPositions { get; set; }
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


		public double GetBallPosition()
		{
			return FinalBallPosition;
		}

		private Dictionary<string, double[]> GetPlinkoPositions(int rows)
		{
			return PlinkoPositions[rows.ToString()];
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
			var file16 = File.ReadAllText("./classes/games/plinko_positions_16.json");
			var file12 = File.ReadAllText("./classes/games/plinko_positions_12.json");
			var file8 = File.ReadAllText("./classes/games/plinko_positions_8.json");
			var plinkoPos16 = JsonSerializer.Deserialize<Dictionary<string, double[]>>(file16);
			var plinkoPos12 = JsonSerializer.Deserialize<Dictionary<string, double[]>>(file12);
			var plinkoPos8 = JsonSerializer.Deserialize<Dictionary<string, double[]>>(file8);

			Dictionary < string, Dictionary<string, double[]>> temp = new Dictionary<string,Dictionary<string, double[]>> ();
			temp.Add("16", plinkoPos16);
			temp.Add("12",plinkoPos12);
			temp.Add("8",plinkoPos8);
			return new PlinkoGame
			{
				BetAmount = gameData.BetAmount,
				CurrentMultiplier = gameData.CurrentMultiplier,
				Status = gameData.Status,
				Type = gameData.GameType,
				FinalBallPosition = ballPos,
				ChoosenDifficulty = diff,
				RowsCount = rows,
				PlinkoPositions = temp
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

			var lessRows = rows + 1;
			var respMult = new decimal[lessRows];
			var respWeight = new int[lessRows];

			for (int i = 0; i < lessRows; i++)
			{
				var pos = (double)i * ((baseMult.Length - 1) - 0.5) / (lessRows - 1);

				var lowIndex = (int)Math.Floor(pos);
				var highIndex = (int)Math.Ceiling(pos);
				var weight = pos - Math.Floor(pos);

				if (i == 0)
				{
					decimal ratio = (decimal)(16 - rows) / 16;
					respMult[i] = baseMult[0] - (baseMult[0] - baseMult[1]) * ratio;
					respWeight[i] = (int)(baseWeight[0] - (baseWeight[0] - baseWeight[1]) * ratio);

				}
				else if (i == lessRows - 1)
				{
					decimal ratio = (decimal)(16 - rows) / 16;
					respMult[i] = baseMult[baseMult.Length - 1] - (baseMult[baseMult.Length - 1] - baseMult[baseMult.Length - 2]) * ratio;
					respWeight[i] = (int)(baseWeight[baseWeight.Length - 1] - (baseWeight[baseWeight.Length - 1] - baseWeight[baseWeight.Length - 2]) * ratio);

				}
				else
				{
					respMult[i] = baseMult[lowIndex] + (decimal)weight * (baseMult[highIndex] - baseMult[lowIndex]);
					respWeight[i] = (int)(baseWeight[lowIndex] + weight * (baseWeight[highIndex] - baseWeight[lowIndex]));
				}


				if (rows < 16)
				{
					if (difficulty == Difficulty.Hard)
					{
						if (rows == 8 && respMult[i] >= 0.2m)
						{
							var scaleFactor = 1m;
							respMult[i] = respMult[i] * scaleFactor;
						}
						else if (rows == 12 && respMult[i] >= 40)
						{
							var scaleFactor = 0.8m;
							respMult[i] = respMult[i] * scaleFactor;
						}
					}
					else if (difficulty == Difficulty.Medium)
					{
						if (rows == 8 && respMult[i] > 70)
						{
							var scaleFactor = 0.4m;
							respMult[i] = respMult[i] * scaleFactor;
						}
						else if (rows == 12 && respMult[i] > 0.3m)
						{
							var scaleFactor = 0.96m;
							respMult[i] = respMult[i] * scaleFactor;
						}
					}
					else if (difficulty == Difficulty.Easy)
					{
						if (rows == 8 && respMult[i] > 1)
						{
							var boostFactor = 1m;
							respMult[i] = respMult[i] * boostFactor;
						}
						else if (rows == 12 && respMult[i] > 1)
						{
							var boostFactor = 0.98m;
							respMult[i] = respMult[i] * boostFactor;
						}

					}
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
			var positions = GetPlinkoPositions(RowsCount);
			double[] pos = positions[index.ToString()];
			var rand = new Random();
			var indexPos = rand.Next(pos.Length);
			return pos[indexPos];
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

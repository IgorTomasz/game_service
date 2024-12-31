

using System.Security.Cryptography;
using System.Text.Json;

namespace game_service.classes.games
{
	public class DiceGame : AbstractGame
	{
		public decimal BetAmount { get; set; }
		public decimal CurrentMultiplier { get; set; }
		public GameStatus Status { get; set; }
		public Guid GameId { get; set; }
		public GameType Type { get; set; }
		public bool CashOutEarly { get; set; }
		private int BettedSum { get; set; }
		private int[] Dices { get; set; }

		private Dictionary<int, decimal> Multipliers = new Dictionary<int, decimal>() {
			{ 2, 17.0m }, 
			{ 3, 8.5m },
			{ 4, 5.5m },
			{ 5, 4.5m },
			{ 6, 3.5m },
			{ 7, 5.5m },
			{ 8, 3.5m },
			{ 9, 4.5m },
			{ 10, 5.5m },
			{ 11, 8.5m },
			{ 12, 17.0m }
		};
		private int DiceSum { get; set; }

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

			var betted = JsonSerializer.Deserialize<int>(gameData.GamesValues["BettedSum"].ToString());
			var dice = JsonSerializer.Deserialize<int>(gameData.GamesValues["DiceSum"].ToString());
			return new DiceGame
			{
				BetAmount = gameData.BetAmount,
				CurrentMultiplier = gameData.CurrentMultiplier,
				Status = gameData.Status,
				GameId = gameData.GameId,
				Type = gameData.GameType,
				BettedSum = betted,
				DiceSum = dice
			};
		}

		public static AbstractGame CreateGame(decimal betAmount)
		{
			Guid guid = Guid.NewGuid();
			DiceGame game = new DiceGame
			{
				BetAmount = betAmount,
				Status = GameStatus.InProgress,
				CurrentMultiplier = 0,
				GameId = guid,
				Type = GameType.Dice
			};
			return game;
		}

		public decimal GetCashWon()
		{
			return BetAmount*CurrentMultiplier;
		}

		private int ThrowDice()
		{
			var rng = RandomNumberGenerator.Create();
			Dices = new int[2];
			for (int i = 0; i < 2; i++)
			{
				byte[] bytes = new byte[4];
				rng.GetBytes(bytes);
				var random = BitConverter.ToInt32(bytes, 0);
				random = Math.Abs(random % 6) + 1;
				DiceSum += random;
				Dices[i] = random;
			}
			return DiceSum;
		}

		public int[] GetDices()
		{
			return Dices;
		}

		public void ValidateMove()
		{
			var sum = ThrowDice();
			if(sum == BettedSum)
			{
				var multi = Multipliers[sum];
				CurrentMultiplier = multi;
				Status = GameStatus.EndedWin;
				return;
			}
			CurrentMultiplier = 0;
			Status = GameStatus.EndedLose;
		}

		public int GetDiceSum()
		{
			return DiceSum;
		}

		public int GetBettedSum()
		{
			return BettedSum;
		}

		public decimal GetWinnedAmount()
		{
			return CurrentMultiplier * BetAmount;
		}

		public void InicializeGame(Dictionary<string, object> gameSettings)
		{
			BettedSum = JsonSerializer.Deserialize<int>(gameSettings["BettedSum"].ToString());
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



using System.Security.Cryptography;
using System.Text.Json;

namespace game_service.classes.games
{
	public class ChickenGame : AbstractGame
	{
		public decimal BetAmount { get; set; }
		public decimal CurrentMultiplier { get; set; }
		public GameStatus Status { get; set; }
		public Guid GameId { get; set; }
		public GameType Type { get; set; }
		public bool CashOutEarly { get; set; }
		private int RandomRoad {  get; set; }
		private int CurrentPosition { get; set; }
		private decimal[] Multipliers = {1.04m, 1.09m,1.14m,1.2m,1.26m,1.33m,1.41m,1.5m,1.6m,1.71m,1.85m,2,2.18m,2.4m,2.67m,3,3.43m,4,4.8m,6,8,12,24};

		public static AbstractGame CreateGame(decimal betAmount)
		{
			Guid guid = Guid.NewGuid();
			ChickenGame game = new ChickenGame
			{
				BetAmount = betAmount,
				Status = GameStatus.InProgress,
				GameId = guid,
				Type = GameType.Chicken,
				CurrentMultiplier = 0,
				CurrentPosition = 0,
				RandomRoad = 0
			};
			return game;
		}

		public static AbstractGame RestoreGameData(GameData gameData)
		{
			var road = JsonSerializer.Deserialize<int>(gameData.GamesValues["RandomRoad"].ToString());
			var position = JsonSerializer.Deserialize<int>(gameData.GamesValues["CurrentPosition"].ToString());
			return new ChickenGame
			{
				BetAmount = gameData.BetAmount,
				Status =gameData.Status,
				GameId =gameData.GameId,
				Type = gameData.GameType,
				CurrentMultiplier = gameData.CurrentMultiplier,
				RandomRoad = road,
				CurrentPosition = position
			};
		}

		public decimal GetCashWon() { return BetAmount*CurrentMultiplier; }

		public decimal GetMultiplier() { return CurrentMultiplier; }

		public GameStatus GetStatus() { return Status; }

		public int GetRandomRoad() { return RandomRoad; }

		public int GetCurrentPosition() {  return CurrentPosition; }

		public bool IsGameOver()
		{
			CurrentPosition++;
			if (CurrentPosition == Multipliers.Length)
			{
				Status = GameStatus.EndedWin;
				CurrentMultiplier = Multipliers[CurrentPosition - 1];
				return true;
			}
			if (CurrentPosition == RandomRoad)
			{
				Status = GameStatus.EndedLose;
				return true;
			}
			CurrentMultiplier=Multipliers[CurrentPosition-1];
			return false;
		}

		public void InicializeGame(Dictionary<string, object> gameSettings)
		{
			var rng = RandomNumberGenerator.Create();
			byte[] bytes = new byte[4];
			rng.GetBytes(bytes);
			var random = BitConverter.ToInt32(bytes, 0);
			RandomRoad = Math.Abs(random%24)+1;
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
	}
}

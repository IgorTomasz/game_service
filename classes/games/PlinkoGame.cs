

using Microsoft.EntityFrameworkCore.Query.Internal;

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
		public Difficulty Difficulty { get; set; }
		public decimal FinalBallPosition { get; set; }
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

		}

		public int CalculateIndex(char[] path)
		{

		}

		public decimal CalculateMultiplier(int index)
		{

		}

		public decimal GetPosition(decimal multiplier)
		{
			

		}

		//trzeba bedzie zmienic typ zwracany
		public void CalculateDrop()
		{
			var path = CalculatePath();
			var index = CalculateIndex(path);
			var multiplier = CalculateMultiplier(index);
			var position = GetPosition(multiplier);
		}

		public void InicializeGame(Dictionary<string, object> gameSettings)
		{
			RowsCount = (int)gameSettings["Rows"];
			Difficulty = (Difficulty)gameSettings["Difficulty"];
		}
	}
}


namespace game_service.classes.games
{
	public class MinesGame : AbstractGame
	{
		public MinesGame(decimal betAmount) 
		{ 
			GameId = Guid.NewGuid();
			Status = GameStatus.InProgress;
			BetAmount = betAmount;
		}

		public decimal BetAmount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public decimal CurrentMultiplier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid GameId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public GameStatus Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public decimal GetMultiplier()
		{
			throw new NotImplementedException();
		}

		public decimal GetResult()
		{
			throw new NotImplementedException();
		}

		public GameStatus GetStatus()
		{
			throw new NotImplementedException();
		}
	}
}

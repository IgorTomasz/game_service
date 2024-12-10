using game_service.classes.games;

namespace game_service.classes
{

	public class GameFactory : AbstractGameFactory
	{
		public AbstractGame CreateGame(GameType gameType, decimal betAmount)
		{
			switch (gameType)
			{
				case GameType.Mines: return new MinesGame(betAmount);
				case GameType.Plinko: return new PlinkoGame(betAmount);
				case GameType.Chicken: return new ChickenGame(betAmount);
				case GameType.Dice: return new DiceGame(betAmount);
				case GameType.BlackJack: return new BlackJackGame(betAmount);
				default: throw new Exception();
			}
		}
	}
}

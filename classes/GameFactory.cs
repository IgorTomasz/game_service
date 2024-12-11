using game_service.classes.games;

namespace game_service.classes
{

	public class GameFactory : AbstractGameFactory
	{
		public static AbstractGame CreateGame(GameType gameType, decimal betAmount)
		{
			switch (gameType)
			{
				case GameType.Mines: return MinesGame.CreateGame(betAmount);
				case GameType.Plinko: return PlinkoGame.CreateGame(betAmount);
				case GameType.Chicken: return ChickenGame.CreateGame(betAmount);
				case GameType.Dice: return DiceGame.CreateGame(betAmount);
				case GameType.BlackJack: return BlackJackGame.CreateGame(betAmount);
				default: throw new Exception();
			}
		}

		public static AbstractGame RestoreGame(GameData gameData)
		{
			switch(gameData.GameType)
			{
				case GameType.Mines: return MinesGame.RestoreGameData(gameData);
				case GameType.Plinko: return PlinkoGame.RestoreGameData(gameData);
				case GameType.Chicken: return ChickenGame.RestoreGameData(gameData);
				case GameType.Dice: return DiceGame.RestoreGameData(gameData);
				case GameType.BlackJack: return BlackJackGame.RestoreGameData(gameData);
				default: throw new Exception();
			};

		}
	}
}

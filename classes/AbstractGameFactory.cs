using game_service.classes.games;

namespace game_service.classes
{
	public interface AbstractGameFactory
	{
		public static abstract AbstractGame CreateGame(GameType gameType, decimal betAmount);
		public static abstract AbstractGame RestoreGame(GameData gameData);
	}
}

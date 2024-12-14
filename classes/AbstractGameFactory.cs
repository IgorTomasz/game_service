using game_service.classes.games;

namespace game_service.classes
{
	public interface AbstractGameFactory
	{
		public static abstract AbstractGame CreateGameFactory(GameType gameType, decimal betAmount);
		public static abstract AbstractGame RestoreGameFactory(GameData gameData);
	}
}

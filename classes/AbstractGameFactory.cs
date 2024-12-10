namespace game_service.classes
{
	public interface AbstractGameFactory
	{
		public AbstractGame CreateGame(GameType gameType, decimal betAmount);
	}
}

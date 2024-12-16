using game_service.classes;
using game_service.classes.games;
using game_service.models;
using game_service.models.DTOs;
using game_service.repositories;
using System.Reflection.Metadata.Ecma335;

namespace game_service.services
{
	public interface IGameService
	{
		public Task<Guid> CreateGameSession(AbstractGame game, Guid userId, Guid userSessionId);
		public AbstractGame GetGame(ProcessGameRequest request);
		public Task CreateGameAction(ProcessGameRequest request, Guid gameSessionId);
		public Task<GameSession> GetGameSession(Guid gameSessionId);
		public AbstractGame RestoreGame(GameData request);
		public Task<object> MakeMove(AbstractGame game, Dictionary<string, object> data);
	}

	public class GameService : IGameService
	{
		private readonly IGameRepository _gameRepository;

		public GameService(IGameRepository gameRepository)
		{
			_gameRepository = gameRepository;
		}

		public async Task<Guid> CreateGameSession(AbstractGame game, Guid userId, Guid userSessionId)
		{
			return await _gameRepository.CreateGameSession(game, userId, userSessionId);
		}

		public async Task<GameSession> GetGameSession(Guid gameSessionId)
		{
			return await _gameRepository.GetGameSession(gameSessionId);
		}

		public AbstractGame GetGame(ProcessGameRequest request)
		{
			return GameFactory.CreateGameFactory(request.Type, request.BetAmount);
		}

		public AbstractGame RestoreGame(GameData request)
		{
			return GameFactory.RestoreGameFactory(request);
		}

		public async Task CreateGameAction(ProcessGameRequest request, Guid gameSessionId)
		{
			await _gameRepository.CreateGameAction(request, gameSessionId);
		}

		public object MakeMove(AbstractGame game, Dictionary<string, object> data)
		{
			switch (game)
			{
				case PlinkoGame plinko:
					{
						plinko.CalculateDrop();
						return new {
							CurrentStatus = plinko.Status,
							Position = plinko.FinalBallPosition,
							Path = plinko.Path,
							Multiplier = plinko.CurrentMultiplier
						};
					}
				case MinesGame mines:
					{

						MinesPosition movePosition = new MinesPosition { X = (int)data["X"], Y = (int)data["Y"] };
						mines.ValidateMove(movePosition);
						var isOver = mines.IsGameOver();
						var multi = mines.CalculateMultiplier();
						if (isOver) {
							return new
							{
								CurrentStatus = mines.Status,
								Fields = mines.field
							};
						}
						return new
						{
							CurrentStatus = mines.Status,
							Multiplier = multi,
						};
					}
				case ChickenGame chicken:
					{
						chicken.ValidateMove();
						break;
					}
				case BlackJackGame blackJack:
					{

					}
			}
		}


	}
}

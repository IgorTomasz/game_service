using game_service.classes;
using game_service.classes.games;
using game_service.models;
using game_service.models.DTOs;
using game_service.repositories;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace game_service.services
{
	public interface IGameService
	{
		public Task<Guid> CreateGameSession(AbstractGame game, Guid userId, Guid userSessionId);
		public AbstractGame GetGame(ProcessGameRequest request);
		public Task CreateGameAction(ProcessGameRequest request, Guid gameSessionId);
		public Task<GameSession> GetGameSession(Guid gameSessionId);
		public AbstractGame RestoreGame(GameData request);
		public GameActionResponse MakeMove(AbstractGame game, Dictionary<string, object> data);
		public GameActionResponse CashOutEarly(AbstractGame game, Dictionary<string, object> data);
		public Task SaveSession(GameSession gameSession);
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

		public GameActionResponse MakeMove(AbstractGame game, Dictionary<string, object> data)
		{
			switch (game)
			{
				case PlinkoGame plinko:
					{
						plinko.CalculateDrop();
						var dict = new Dictionary<string, object>();
						dict["Position"] = plinko.FinalBallPosition;
						dict["Path"] = plinko.Path;
						return new GameActionResponse
						{
							Status = plinko.Status,
							Multiplier = plinko.CurrentMultiplier,
							Data = dict

						};
					}
				case MinesGame mines:
					{
						var X = JsonSerializer.Deserialize<int>(data["X"].ToString());
						var Y = JsonSerializer.Deserialize<int>(data["Y"].ToString());

						MinesPosition movePosition = new MinesPosition { X = X, Y = Y };
						var isOver = mines.ValidateMove(movePosition);
						if (isOver) {
							var dict = new Dictionary<string, object>();
							dict["Fields"] = mines.Field;
							return new GameActionResponse
							{
								Status = mines.Status,
								Data = dict
							};
						}
						return new GameActionResponse
						{
							Status = mines.Status,
							Multiplier = mines.CurrentMultiplier,
						};
					}
				case ChickenGame chicken:
					{
						var isGameOver = chicken.IsGameOver();
						var dict = new Dictionary<string, object>();
						dict["GameOver"] = isGameOver;
						return new GameActionResponse
						{
							Status = chicken.Status,
							Multiplier = chicken.CurrentMultiplier,
							Data = dict
						};
					}
				default: return null;
			
		
/*
				case BlackJackGame blackJack:
					{

					}
*/
			}
		}

		public GameActionResponse CashOutEarly(AbstractGame game, Dictionary<string, object> data)
		{
			switch (game)
			{
				case MinesGame mines:
					{
						mines.CashOutEarly = true;
						mines.Status = GameStatus.EndedWin;
						var dict = new Dictionary<string, object>();
						dict["Fields"] = mines.Field;
						dict["Result"] = mines.BetAmount*mines.CurrentMultiplier;
						return new GameActionResponse
						{
							Status = mines.Status,
							Multiplier = mines.CurrentMultiplier,
							Data = dict
						};
					}
				case ChickenGame chicken:
					{
						chicken.CashOutEarly = true;
						chicken.Status = GameStatus.EndedWin;
						var dict = new Dictionary<string, object>();
						dict["Result"] = chicken.BetAmount * chicken.CurrentMultiplier;
						return new GameActionResponse
						{
							Status = chicken.Status,
							Multiplier = chicken.CurrentMultiplier,
							Data = dict
						};

					}
				default: return null;
			}
		}

		public async Task SaveSession(GameSession gameSession)
		{
			await _gameRepository.SaveSession(gameSession);
		}


	}
}


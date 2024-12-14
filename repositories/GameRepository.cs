using game_service.classes;
using game_service.context;
using game_service.models;
using game_service.models.DTOs;

namespace game_service.repositories
{
	public interface IGameRepository
	{
		public Task<Guid> CreateGameSession(AbstractGame game, Guid userId, Guid userSessionId);
		public Task CreateGameAction(ProcessGameRequest request, Guid gameSessionId);
		public Task<GameSession> GetGameSession(Guid gameSessionId);
	}

	public class GameRepository : IGameRepository
	{
		private readonly GameDatabaseContext _context;

		public GameRepository(GameDatabaseContext context)
		{
			_context = context;

		}

		public async Task<Guid> CreateGameSession(AbstractGame game, Guid userId, Guid userSessionId)
		{
			GameSession gameSession = new GameSession
			{
				GameSessionId = Guid.NewGuid(),
				GameId = game.GameId,
				UserId = userId,
				UserSessionId = userSessionId,
				GameHistoryId = null,
				BetAmount = game.BetAmount,
				GameType = game.Type,
				Result = null,
				CashWon = 0,
				StartTime = DateTime.UtcNow.AddHours(1),
				EndTime = null,
				Status = game.Status,
				CurrentMultiplier = game.CurrentMultiplier,
				CashedOutEarly = game.CashOutEarly,
				Game = game,
			};

			await _context.GameSessions.AddAsync(gameSession);
			await _context.SaveChangesAsync();
			return gameSession.GameSessionId;
		}

		public async Task CreateGameAction(ProcessGameRequest request, Guid gameSessionId)
		{
			GameAction action = new GameAction
			{
				GameActionId = Guid.NewGuid(),
				GameSessionId = gameSessionId,
				Timestamp = DateTime.UtcNow.AddHours(1),
				Type = request.Action,
				ActionData = request.Data
			};

			await _context.GameActions.AddAsync(action);
			await _context.SaveChangesAsync();


		}

		public async Task<GameSession> GetGameSession(Guid gameSessionId)
		{
			return await _context.GameSessions.FindAsync(gameSessionId);
		}
	}
}

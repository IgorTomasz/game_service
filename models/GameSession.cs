using game_service.classes;
using game_service.classes.games;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace game_service.models
{
	public class GameSession
	{
		[Key]
		public Guid GameSessionId { get; set; }
		[Required]
		public Guid UserId { get; set; }
		[Required] 
		public Guid UserSessionId { get; set; }
		public Guid? GameHistoryId { get; set; }
		[Required]
		public decimal BetAmount { get; set; }
		[Required]
		public GameType GameType { get; set; }
		public ResultType? Result {  get; set; }
		[Required]
		public decimal CashWon {  get; set; }
		[Required]
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		[Required]
		public GameStatus Status { get; set; }
		[Required]
		public decimal CurrentMultiplier { get; set; }
		[Required]
		public bool CashedOutEarly { get; set; }
		public virtual ICollection<GameAction> GameActions { get; set; }
		[ForeignKey(nameof(GameHistoryId))]
		public virtual GameHistory History { get; set; }
		[Required]
		public string SerializedGame {  get; set; }
		[NotMapped]
		public AbstractGame Game { get => DeserializeGameData(); set=> SerializedGame = SerializeGame(value); }

		private string SerializeGame(AbstractGame game)
		{
			var data = new GameData
			{
				GameType = this.GameType,
				BetAmount = game.BetAmount,
				GameId = game.GetGameId(),
				CurrentMultiplier = game.CurrentMultiplier,
				Status = game.Status,
				GamesValues = GetGameValues(game),
			};

			return JsonSerializer.Serialize(data);
		}

		private AbstractGame DeserializeGameData()
		{

			var game = JsonSerializer.Deserialize<GameData>(SerializedGame);
			return GameFactory.RestoreGameFactory(game);
		}

		public Dictionary<string, object> GetGameValues(AbstractGame game)
		{
			var data = new Dictionary<string, object>();

			switch (game)
			{
				case MinesGame mines:
					data["Field"] = mines.GetField();
					data["MinesCount"] = mines.GetMinesCount();
					data["DiscoveredDiamonds"] = mines.GetDiscoveredDiamonds(); break;
				case DiceGame dice:
					data["DiceSum"] = dice.GetDiceSum();
					data["BettedSum"] = dice.GetBettedSum();
					break;
				case PlinkoGame plinko:
					data["Rows"] = plinko.GetRowsCount();
					data["Difficulty"] = plinko.GetChoosenDifficulty();
					data["FinalBallPosition"] = plinko.GetBallPosition();
					data["PlinkoPositions"] = plinko.GetPlinkoPositions();
					data["Path"] = plinko.GetPath(); break;
				case ChickenGame chicken:
					data["RandomRoad"] = chicken.GetRandomRoad();
					data["CurrentPosition"] = chicken.GetCurrentPosition();
					break;
				case BlackJackGame blackJack:
					data["DealerHand"] = blackJack.GetDealerHand();
					data["DealerSum"] = blackJack.GetDealerSum();
					data["DealerAceSum"] = blackJack.GetDealerAceSum();
					data["DealerHiddenCard"] = blackJack.GetDealerHidden();
					data["PlayerHand"] = blackJack.GetPlayerHand();
					data["PlayerSum"] = blackJack.GetPlayerSum();
					data["PlayerAceSum"] = blackJack.GetPlayerAceSum();
					data["Cards"] = blackJack.GetCards();
					data["DealerSumWithHidden"] = blackJack.GetDealerSumWithHidden();
					break;
			}

			return data;
		}
	}
	public enum ResultType
	{
		Won, Lost
	}
}

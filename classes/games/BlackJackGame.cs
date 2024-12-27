

using System.Text.Json;

namespace game_service.classes.games
{
	public class Card
	{
		public string Value { get; set; }
		public string Type {get; set; }

		public Card(string value, string type)
		{
			Value = value;
			Type = type;
		}

		public int GetCardValue()
		{
			if ("AJQK".Contains(Value))
			{
				if (Value == "A")
				{
					return 11;
				}
				return 10;
			}
			return int.Parse(Value);
		}

		public bool IsCardAce()
		{
			if (Value.Contains("A"))
			{
				return true;
			}
			return false;
		}

		public string GetCardType()
		{
			return Type;
		}

		public string ToString()
		{
			return Value + " - " + Type;
		}
	}
	public class BlackJackGame : AbstractGame
	{


		public decimal BetAmount { get; set; }
		public decimal CurrentMultiplier { get; set; }
		public GameStatus Status { get; set; }
		public Guid GameId { get; set; }
		public GameType Type { get; set; }
		public bool CashOutEarly { get; set; }
		private List<Card> Cards;
		private string[] Values = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
		private string[] Types = { "C", "D", "H", "S" };

		private decimal _blackjack = 2.5m;
		private decimal _normal = 2.0m;
		private decimal _tie = 1.0m;

		//Dealer
		private int DealerSum {  get; set; }
		private int DealerSumWithHidden { get; set; }
		private Card DealerHidden { get; set; }
		private List<Card> DealerHand {  get; set; }
		private int DealerAceSum { get; set; }

		//Player
		private int PlayerSum { get; set; }
		private List<Card> PlayerHand { get; set; }
		private int PlayerAceSum { get; set; }

		public List<Card> GetCards()
		{
			return Cards;
		}
		public int GetPlayerSum()
		{
			return PlayerSum;
		}
		public List<Card> GetPlayerHand()
		{
			return PlayerHand;
		}
		public int GetPlayerAceSum()
		{
			return PlayerAceSum;
		}

		public List<Card> GetDealerHand()
		{
			return DealerHand;
		}
		public int GetDealerAceSum()
		{
			return DealerAceSum;
		}
		public int GetDealerSumWithHidden()
		{
			return DealerSumWithHidden;
		}
		public int GetDealerSum()
		{
			return DealerSum;
		}
		public Card GetDealerHidden()
		{
			return DealerHidden;
		}

		public decimal GetMultiplier()
		{
			return CurrentMultiplier;
		}

		public GameStatus GetStatus()
		{
			return Status;
		}

		public static AbstractGame RestoreGameData(GameData gameData)
		{
			var playerHand = JsonSerializer.Deserialize<List<Card>>(gameData.GamesValues["PlayerHand"].ToString());
			var playerSum = JsonSerializer.Deserialize<int>(gameData.GamesValues["PlayerSum"].ToString());
			var playerAce = JsonSerializer.Deserialize<int>(gameData.GamesValues["PlayerAceSum"].ToString());
			var dealerHand = JsonSerializer.Deserialize<List<Card>>(gameData.GamesValues["DealerHand"].ToString());
			var dealerSum = JsonSerializer.Deserialize<int>(gameData.GamesValues["DealerSum"].ToString());
			var dealerAce = JsonSerializer.Deserialize<int>(gameData.GamesValues["DealerAceSum"].ToString());
			var dealerHidden = JsonSerializer.Deserialize<Card>(gameData.GamesValues["DealerHiddenCard"].ToString());
			var cards = JsonSerializer.Deserialize<List<Card>>(gameData.GamesValues["Cards"].ToString());
			var dealerSumWithHidden = JsonSerializer.Deserialize<int>(gameData.GamesValues["DealerSumWithHidden"].ToString());

			return new BlackJackGame
			{
				GameId = gameData.GameId,
				Status = gameData.Status,
				Type = gameData.GameType,
				BetAmount = gameData.BetAmount,
				PlayerHand = playerHand,
				PlayerSum = playerSum,
				PlayerAceSum = playerAce,
				DealerAceSum = dealerAce,
				DealerSum = dealerSum,
				DealerHand = dealerHand,
				DealerHidden = dealerHidden,
				Cards = cards,
				DealerSumWithHidden = dealerSumWithHidden,
			};
		}

		public static AbstractGame CreateGame(decimal betAmount)
		{
			Guid guid = Guid.NewGuid();
			BlackJackGame game = new BlackJackGame
			{
				GameId = guid,
				Status = GameStatus.InProgress,
				BetAmount = betAmount,
				Type = GameType.BlackJack
			};
			return game;
		}

		public decimal GetCashWon()
		{
			return BetAmount*CurrentMultiplier; 
		}

		public void InicializeGame(Dictionary<string, object> gameSettings)
		{
			Cards = new List<Card>();
			DealerHand = new List<Card>();
			PlayerHand = new List<Card>();
			CreateCards();
			ShuffleCards();
			CashOutEarly = false;
			DealerSum = 0;
			DealerAceSum = 0;
			DealerSumWithHidden = 0;
			DealerHidden = Cards.ElementAt(Cards.Count()-1);
			Cards.Remove(DealerHidden);
			DealerSumWithHidden += DealerHidden.GetCardValue();
			DealerAceSum += DealerHidden.IsCardAce() ? 1 : 0;
			var card = Cards.ElementAt(Cards.Count() - 1);
			Cards.Remove(card);
			DealerSumWithHidden += card.GetCardValue();
			DealerSum += card.GetCardValue();
			DealerAceSum += card.IsCardAce() ? 1 : 0;
			DealerHand.Add(card);

			PlayerSum = 0;
			PlayerAceSum = 0;
			var playerCard1 = Cards.ElementAt(Cards.Count() - 1);
			Cards.Remove(playerCard1);
			PlayerHand.Add(playerCard1);
			PlayerSum += playerCard1.GetCardValue();
			PlayerAceSum = playerCard1.IsCardAce() ? 1 : 0;
			var playerCard2 = Cards.ElementAt(Cards.Count() - 1);
			Cards.Remove(playerCard2);
			PlayerHand.Add(playerCard2);
			PlayerSum += playerCard2.GetCardValue();
			PlayerAceSum = playerCard2.IsCardAce() ? 1 : 0;

		}

		private void CreateCards()
		{
			for (int i = 0; i < Values.Count(); i++)
			{
				for (int j = 0; j < Types.Count(); j++)
				{
					Cards.Add(new Card(Values[i], Types[j]));
				}
			}
		}

		public void CheckPlayerCardSum()
		{
			if (PlayerSum == 21)
			{
				Status = GameStatus.EndedWin;
				CurrentMultiplier = _blackjack;
			}
			else if (PlayerSum > 21)
			{
				Status = GameStatus.EndedLose;
				CurrentMultiplier = 0;
			}
		}

		public void CheckEndGameSum()
		{
			if (DealerSumWithHidden > PlayerSum)
			{
				Status = GameStatus.EndedLose;
				CurrentMultiplier = 0;
			}
			else if (DealerSumWithHidden == PlayerSum)
			{
				Status = GameStatus.EndedWin;
				CurrentMultiplier = _tie;
			}
			else
			{
				Status = GameStatus.EndedWin;
				CurrentMultiplier = _normal;
			}
		}

		public void CheckDealerCardSum()
		{
			if (DealerSumWithHidden == 21)
			{
				Status = GameStatus.EndedLose;
				CurrentMultiplier = 0;
			}
			else if (DealerSumWithHidden > 21)
			{
				Status = GameStatus.EndedWin;
				CurrentMultiplier = _normal;
			}
		}

		public void RemovePlayerAce()
		{
			if(PlayerAceSum > 0 && PlayerSum > 21)
			{
				PlayerSum -= 10;
				PlayerAceSum -= 1;
			}
		}

		public void RemoveDealerAce()
		{
			if (DealerAceSum > 0 && DealerSumWithHidden > 21)
			{
				DealerSumWithHidden -= 10;
				DealerAceSum -= 1;
			}
		}

		public void HitPlayer()
		{
			var playerCard = Cards.ElementAt(Cards.Count() - 1);
			Cards.Remove(playerCard);
			PlayerHand.Add(playerCard);
			PlayerSum += playerCard.GetCardValue();
			PlayerAceSum = playerCard.IsCardAce() ? 1 : 0;
			RemovePlayerAce();
			CheckPlayerCardSum();
		}

		private void HitDealer()
		{
			var card = Cards.ElementAt(Cards.Count() - 1);
			Cards.Remove(card);
			DealerSumWithHidden += card.GetCardValue();
			DealerAceSum += card.IsCardAce() ? 1 : 0;
			DealerHand.Add(card);
			RemoveDealerAce();
			CheckDealerCardSum();
		}

		public void Stand()
		{
			CashOutEarly = true;
			DealerHand.Add(DealerHidden);
			CheckDealerCardSum();
			while (DealerSumWithHidden < 17)
			{
				HitDealer();
			}
			if(Status == GameStatus.InProgress)
			{
				CheckEndGameSum();
			}
			
		}

		public void ShuffleCards()
		{
			Cards = Cards.OrderBy(x => Random.Shared.Next()).ToList();
		}

		public void CashOut()
		{
			throw new NotImplementedException();
		}

		public Guid GetGameId()
		{
			return GameId;
		}

		public decimal GetWinnedAmount()
		{
			return CurrentMultiplier * BetAmount;
		}
	}

	public enum PlayerMove
	{
		Hit, Stand, Info
	}
}

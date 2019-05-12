using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fool
{
    public enum Players
    {
        Bot,
        Gamer
    }

    public class GameModel
    {
        public List<Card> GamerHand { get; }
        public Queue<Card> Deck { get; }
        public List<Card> BotHand { get; }
        public List<DeskCardsSlot> DeskCards { get; set; }
        public Players WhosTurn { get; private set; }
        public Card TrumpCard { get; }
        private Random rnd = new Random();
        
        public GameModel()
        {
            Deck = new Queue<Card>();
            var fromCleanDeck = Card.CleanDeck.ToList();
            while (Deck.Count != 36)
            {
                var number = rnd.Next(0, 35 - Deck.Count);
                if (Deck.Contains(fromCleanDeck[number])) continue;
                Deck.Enqueue(fromCleanDeck[number]);
                fromCleanDeck.RemoveAt(number);
            }

            GamerHand = new List<Card>();
            BotHand = new List<Card>();

            for (var i = 0; i < 6; i++)
            {
                GamerHand.Add(Deck.Dequeue());
                BotHand.Add(Deck.Dequeue());
            }

            TrumpCard = Deck.Dequeue();
            Deck.Enqueue(TrumpCard);
            WhosTurn = WhoFirst();
            if (WhosTurn == Players.Bot)
                TurnBot2Player();
        }

        public GameModel(List<Card> botHand, List<Card> gamerHand, Queue<Card> deck, List<DeskCardsSlot> deskCards, Card trumpCard, Players whosTurn)
        {
            GamerHand = gamerHand;
            Deck = deck;
            BotHand = botHand;
            DeskCards = deskCards;
            WhosTurn = whosTurn;
            TrumpCard = trumpCard;
        }

        public Tuple<string, string> CloseBotCard(int gamerCardNumber)
        {
            if (!IsClosingRight(GamerHand[gamerCardNumber], DeskCards[0].Back))
                return new Tuple<string, string>("Нарушение правил", "Неправильно выбрана карта");
            DeskCards[0].Close(GamerHand[gamerCardNumber]);
            GamerHand.RemoveAt(gamerCardNumber);
            return null;
        }

        public Tuple<string, string> TurnBot2Player()
        {
            var cardFound = false;
            foreach (var card in BotHand)
            {
                if (card.Suit == TrumpCard.Suit) continue;
                DeskCards = new List<DeskCardsSlot> { new DeskCardsSlot(card) };
                BotHand.Remove(card);
                cardFound = true;
                break;
            }

            if (!cardFound)
            {
                DeskCards.Add(new DeskCardsSlot(BotHand[0]));
                BotHand.RemoveAt(0);
            }

            var IsGamerCanClose = false;

            foreach (var card in GamerHand)
            {
                if (!IsClosingRight(card, DeskCards[0].Back)) continue;
                IsGamerCanClose = true;
                break;
            }

            return IsGamerCanClose
                ? null
                : new Tuple<string, string>("Упс", "Вам нечем покрыть карту противника");
        }

        public void TurnPlayer2Bot(int gamerCardNumber)
        {
            DeskCards = new List<DeskCardsSlot> { new DeskCardsSlot(GamerHand[gamerCardNumber]) };
            GamerHand.RemoveAt(gamerCardNumber);
        }

        public void CloseGamerCard()
        {
            foreach (var card in BotHand)
            {
                if (!IsClosingRight(card, DeskCards[0].Back)) continue;
                DeskCards[0].Close(card);
                BotHand.Remove(card);
                break;
            }
        }

        public void CloseTurn()
        {
            if (DeskCards[0].ContainsFore)
            {
                if (WhosTurn == Players.Gamer)
                {
                    DeskCards.Clear();
                    AddCardsToGamersHands();
                    WhosTurn = Players.Bot;
                }
                else
                {
                    DeskCards.Clear();
                    AddCardsToGamersHands();
                    WhosTurn = Players.Gamer;
                }
            }
            else if (DeskCards[0].ContainsFore == false)
            {
                if (WhosTurn == Players.Gamer)
                {
                    BotHand.Add(DeskCards[0].Back);
                    DeskCards.Clear();
                    AddCardsToGamersHands();
                }
                else
                {
                    GamerHand.Add(DeskCards[0].Back);
                    DeskCards.Clear();
                    AddCardsToGamersHands();
                }
            }
        }

        public bool IsClosingRight(Card fore, Card back)
        {
            return back.Suit != TrumpCard.Suit
                ? (fore.Rank > back.Rank && fore.Suit == back.Suit) || fore.Suit == TrumpCard.Suit
                : fore.Suit == TrumpCard.Suit && fore.Rank > back.Rank;
        }

        private Players WhoFirst()
        {
            var rank = 0;
            var result = Players.Bot;
            foreach (var card in BotHand)
                if (card.Suit == TrumpCard.Suit && (rank == 0 || card.Rank < rank))
                {
                    result = Players.Bot;
                    rank = card.Rank;
                }

            foreach (var card in GamerHand)

                if (card.Suit == TrumpCard.Suit && (rank == 0 || card.Rank < rank))
                {
                    result = Players.Gamer;
                    rank = card.Rank;
                }
            return result;
        }

        private void AddCardsToGamersHands()
        {
            GamerHand.Remove(null);
            BotHand.Remove(null);
            if (Deck == null)
                return;
            if (WhosTurn == Players.Gamer)
            {
                while (GamerHand.Count < 6 && Deck.Count != 0)
                {
                    GamerHand.Add(Deck.Dequeue());
                    GamerHand.Remove(null);
                }

                while (BotHand.Count < 6 && Deck.Count != 0)
                {
                    BotHand.Add(Deck.Dequeue());
                    BotHand.Remove(null);
                }
            }
            else if (WhosTurn == Players.Bot)
            {
                while (BotHand.Count < 6 && Deck.Count != 0)
                {
                    BotHand.Add(Deck.Dequeue());
                    BotHand.Remove(null);
                }

                while (GamerHand.Count < 6 && Deck.Count != 0)
                {
                    GamerHand.Add(Deck.Dequeue());
                    GamerHand.Remove(null);
                }
            }
        }
    }

}

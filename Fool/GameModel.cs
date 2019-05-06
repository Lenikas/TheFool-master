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
        public List<Card> GamerHand { get; set; }
        public Queue<Card> Deck { get; private set; }
        public List<Card> BotHand { get; set; }
        public DeskCardsSlot DeskCards { get; set; }
        public Players WhosTurn { get; private set; }
        public Card TrumpCard { get; private set; }
        private Random rnd = new Random();
        public bool FirstBotTurn;

        public GameModel()
        {
            Deck = new Queue<Card>();
            var fromCleanDeck = Card.CleanDeck.ToList();
            while(Deck.Count != 36)
            {
                var number = rnd.Next(0, 35 - Deck.Count);
                if (!Deck.Contains(fromCleanDeck[number]))
                {
                    Deck.Enqueue(fromCleanDeck[number]);
                    fromCleanDeck.RemoveAt(number);
                }       
            }

            GamerHand = new List<Card>();
            BotHand = new List<Card>();

            for(int i = 0; i < 6; i++)
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

        public GameModel(List<Card> botHand, List<Card> gamerHand, Queue<Card> deck, DeskCardsSlot deskCards, Card trumpCard, Players whosTurn)
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
            if (IsClosingRight(GamerHand[gamerCardNumber], DeskCards.Back))
            {
                DeskCards.Close(GamerHand[gamerCardNumber]);
                GamerHand.RemoveAt(gamerCardNumber);
                return null;
            }
            return new Tuple<string, string>("Нарушение правил", "Неправильно выбрана карта");    
        }

        public Tuple<string, string> TurnBot2Player()
        {
            var cardFound = false;
            foreach(var card in BotHand)
            {
                if (card.Suit != TrumpCard.Suit)
                {
                    DeskCards = new DeskCardsSlot(card);
                    BotHand.Remove(card);
                    cardFound = true;
                    break;
                }
            }

            if(!cardFound)
            {
                DeskCards = new DeskCardsSlot(BotHand[0]);
                BotHand.RemoveAt(0);
            }

            var IsGamerCanClose = false;

            foreach (var card in GamerHand)
            {
                if (IsClosingRight(card, DeskCards.Back))
                {
                    IsGamerCanClose = true;
                    break;
                }
            }

            return IsGamerCanClose
                ? null
                : new Tuple<string, string>("Упс", "Вам нечем покрыть карту противника");
        }

        public void TurnPlayer2Bot(int gamerCardNumber)
        {
            DeskCards = new DeskCardsSlot(GamerHand[gamerCardNumber]);
            GamerHand.RemoveAt(gamerCardNumber);
        }

        public void CloseGamerCard()
        {
            foreach(var card in BotHand)
            {
                if(IsClosingRight(card, DeskCards.Back))
                {
                    DeskCards.Close(card);
                    BotHand.Remove(card);
                    break;
                }
            }
        }

        public void CloseTurn()
        {
            if (DeskCards.Closed == true)
            {
                if(WhosTurn == Players.Gamer)
                {
                    DeskCards.Clear();

                    try
                    {
                        BotHand.Add(Deck.Dequeue());
                        GamerHand.Add(Deck.Dequeue());
                    }
                    catch (InvalidOperationException)
                    {

                    }

                    WhosTurn = Players.Bot;
                }
                else
                {
                    DeskCards.Clear();
                    try
                    {
                        BotHand.Add(Deck.Dequeue());
                        GamerHand.Add(Deck.Dequeue());
                    }
                    catch(InvalidOperationException)
                    {

                    }
                    WhosTurn = Players.Gamer;
                }
            }
            else
            {
                if (WhosTurn == Players.Gamer)
                {
                    BotHand.Add(DeskCards.Back);
                    DeskCards.Clear();
                    try
                    {
                        GamerHand.Add(Deck.Dequeue());
                    }
                    catch (InvalidOperationException)
                    {

                    }
                }
                else
                {
                    GamerHand.Add(DeskCards.Back);
                    DeskCards.Clear();
                    try
                    {
                        BotHand.Add(Deck.Dequeue());
                    }
                    catch (InvalidOperationException)
                    {

                    }
                }
            }


        }

        public bool IsClosingRight (Card fore, Card back)
        {
            return back.Suit != TrumpCard.Suit 
                ? (fore.Rank > back.Rank && fore.Suit == back.Suit) || fore.Suit == TrumpCard.Suit 
                : fore.Suit == TrumpCard.Suit && fore.Rank > back.Rank;
        }

        private Players WhoFirst()
        {
            var rank = 0;
            Players result = Players.Bot;
            foreach(var card in BotHand)
                if (card.Suit == TrumpCard.Suit && (rank == 0 || card.Rank < rank))
                {
                    result = Players.Bot;
                    rank = card.Rank;
                }

            foreach(var card in GamerHand)

                if (card.Suit == TrumpCard.Suit && (rank == 0 || card.Rank < rank))
                {
                    result = Players.Gamer;
                    rank = card.Rank;
                }
            return result;
        }
    }

}

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

            //if (CanTransferPlayer(gamerCardNumber))
            //{
            //    DoTransferPlayer(gamerCardNumber);
            //    return null;
            //}
            foreach (var cardDesk in DeskCards)
            {
                if (IsClosingRight(GamerHand[gamerCardNumber], cardDesk.Back))
                {
                    cardDesk.Close(GamerHand[gamerCardNumber]);
                    GamerHand.RemoveAt(gamerCardNumber);
                }
                else
                {
                    return new Tuple<string, string>("Нарушение правил", "Неправильно выбрана карта");
                }
            }

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
        //public void TryCloseAnyGamerCard()
        //{
        //    var i = DeskCards.Count;
        //    foreach (var cardDesk in DeskCards)
        //    {
        //        foreach (var card in BotHand)
        //        {
        //            if (IsClosingRight(card, cardDesk.Back))
        //            {
        //                cardDesk.Close(card);
        //                BotHand.Remove(card);
        //                i--;
        //                break;
        //            }
        //        }
        //    }

        //    if (i != 0)
        //    {
        //        foreach (var card in DeskCards)
        //        {
        //            BotHand.Add(card.Back);
        //            if (card.Fore != null)
        //                BotHand.Add(card.Fore);
        //        }
        //        Thread.Sleep(1000);
        //        DeskCards.Clear();
        //        AddCardsToGamersHands();
        //    }

        //    if (i == 0)
        //    {
        //        DeskCards.Clear();
        //        AddCardsToGamersHands();  
        //        WhosTurn = Players.Bot;
        //    }
        //}

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
                DoDistribute(GamerHand, BotHand);
            }

            if (WhosTurn == Players.Bot)
            {
                DoDistribute(BotHand, GamerHand);
            }
        }

        private void DoDistribute(List<Card> handOne, List<Card> handTwo)
        {
            while (handOne.Count < 6 && Deck.Count != 0)
            {
                handOne.Add(Deck.Dequeue());
                handOne.Remove(null);
            }
            while (handTwo.Count < 6 && Deck.Count != 0)
            {
                handTwo.Add(Deck.Dequeue());
                handTwo.Remove(null);
            }
        }

        public bool CanTransferBot()
        {
            var cardForTransfer = DeskCards.Last().Back;
            foreach (var card in BotHand)
                if (card.Rank == cardForTransfer.Rank)
                    return true;

            return false;
        }

        public void DoTransferBot()
        {
           
            var cardForTransfer = DeskCards.Last().Back;
            foreach (var card in BotHand)
                if (card.Rank == cardForTransfer.Rank)
                {
                    DeskCards.Add(new DeskCardsSlot(card));
                    BotHand.Remove(card);
                    WhosTurn = Players.Bot;
                    break;
                }
        }

        public bool CanTransferPlayer(int gamerCardNumber)
        {
            var cardForTransfer = DeskCards.Last().Back;
            if (GamerHand[gamerCardNumber].Rank == cardForTransfer.Rank)
                return true;
            return false;
        }

        public void DoTransferPlayer(int gamerCardNumber)
        {
            
            DeskCards.Add(new DeskCardsSlot(GamerHand[gamerCardNumber]));
            GamerHand.RemoveAt(gamerCardNumber);
            // WhosTurn = Players.Bot;
        }

        public bool CanBotToss()
        {
            foreach (var cardDesk in DeskCards)
            {
                foreach (var cardBot in BotHand)

                {
                    if (cardDesk.Fore.Rank == cardBot.Rank || cardDesk.Back.Rank == cardBot.Rank)
                    {
                        return true;
                    }
                }
            }


            return false;
        }

        public void DoTossBot()
        {
            foreach (var cardDesk in DeskCards)

            {
                foreach (var cardBot in BotHand)
                {
                    if (cardDesk.Fore.Rank == cardBot.Rank || cardDesk.Back.Rank == cardBot.Rank)
                    {
                        if (DeskCards.Count < 5)
                        {
                            DeskCards.Add(new DeskCardsSlot(cardBot));
                            BotHand.Remove(cardBot);
                            break;
                        }
                    }
                }

            }
        }

        public bool CanPlayerToss(int cardNumber)
        {
            foreach (var cardDesk in DeskCards)
            {
                if (cardDesk.Back.Rank == GamerHand[cardNumber].Rank ||
                    cardDesk.Fore.Rank == GamerHand[cardNumber].Rank)
                    return true;
            }

            return false;
        }

        public void DoPlayerToss(int cardNumber)
        {
            DeskCards.Add(new DeskCardsSlot(GamerHand[cardNumber]));
            GamerHand.RemoveAt(cardNumber);
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Fool
{
    [TestFixture()]
    public class NUnitTestClass
    {
        private List<Card> botHand = new List<Card> { Card.CleanDeck[6] };
        private List<Card> gamerHand = new List<Card> { Card.CleanDeck[10] };
        private Queue<Card> deck = new Queue<Card>();

        [Test]
        public void TestRightClosingCard2Card()
        {
            deck.Enqueue(Card.CleanDeck[5]);
            deck.Enqueue(Card.CleanDeck[4]);
            
            var whosTurn = Players.Gamer;
            var trumpCard = Card.CleanDeck[1];
            var desk = new List<DeskCardsSlot> { new DeskCardsSlot(Card.CleanDeck[15]) };
            var game = new GameModel(botHand, gamerHand, deck, desk, trumpCard, whosTurn);

            game.CloseBotCard(0);
            game.CloseTurn();

            Assert.AreEqual(2, game.BotHand.Count());
            Assert.AreEqual(2, game.GamerHand.Count());

        }

        [Test]
        public void TestRightNotAbleToClose()
        {
            botHand = new List<Card> { Card.CleanDeck[10] };
            gamerHand = new List<Card> { Card.CleanDeck[6] };

            botHand.Add(Card.CleanDeck[15]);
            deck.Enqueue(Card.CleanDeck [5]);
            deck.Enqueue(Card.CleanDeck [4]);

            var whosTurn = Players.Bot;
            var trumpCard = Card.CleanDeck [3];
            var desk = new List<DeskCardsSlot> { new DeskCardsSlot(null) };
            desk.Clear();
            var game = new GameModel(botHand, gamerHand, deck, desk, trumpCard, whosTurn);

            var result = game.TurnBot2Player();

            Assert.AreEqual("Упс", result.Item1);
        }

        [Test]
        public void TestTakeCards()
        {
            botHand = new List<Card> { Card.CleanDeck[10] };
            gamerHand = new List<Card> { Card.CleanDeck[6] };

            botHand.Add(Card.CleanDeck[15]);
            deck.Enqueue(Card.CleanDeck[5]);
            deck.Enqueue(Card.CleanDeck[4]);

            var whosTurn = Players.Bot;
            var trumpCard = Card.CleanDeck[3];
            var desk = new List<DeskCardsSlot> { new DeskCardsSlot(null) };
            desk.Clear();
            var game = new GameModel(botHand, gamerHand, deck, desk, trumpCard, whosTurn);

            game.TurnBot2Player();
            game.CloseTurn();

            Assert.AreEqual(2, gamerHand.Count);
        }

        [Test]
        public void TestWrongClose()
        {
            botHand = new List<Card> { Card.CleanDeck[10] };
            gamerHand = new List<Card> { Card.CleanDeck[6] };

            gamerHand.Add(Card.CleanDeck[15]);
            deck.Enqueue(Card.CleanDeck [5]);
            deck.Enqueue(Card.CleanDeck [4]);

            var whosTurn = Players.Gamer;
            var trumpCard = Card.CleanDeck [0];
            var desk = new List<DeskCardsSlot> { new DeskCardsSlot(null) };
            desk.Clear();
            var game = new GameModel(botHand, gamerHand, deck, desk, trumpCard, whosTurn);

            game.TurnBot2Player();
            var result = game.CloseBotCard(1);

            Assert.AreEqual("Нарушение правил", result.Item1);
        }

        [Test]
        public void TestRightTransferPlayerToBot()
        {
            botHand = new List<Card> { Card.CleanDeck[0] };
            gamerHand = new List<Card> { Card.CleanDeck[1] };

            var whosTurn = Players.Bot;
            var trumpCard = Card.CleanDeck[2];
            var desk = new List<DeskCardsSlot> { new DeskCardsSlot(null) };
            desk.Clear();
            var game = new GameModel(botHand, gamerHand, deck, desk, trumpCard, whosTurn);

            game.TurnBot2Player();
            var canTransfer = game.CanTransferPlayer(0);
            game.DoTransferPlayer(0);

            Assert.AreEqual(true, canTransfer);
            Assert.AreEqual(2, game.DeskCards.Count);
        }

        [Test]
        public void TestRightTransferBotToPlayer()
        {
            botHand = new List<Card> { Card.CleanDeck[0] };
            gamerHand = new List<Card> { Card.CleanDeck[1] };

            var whosTurn = Players.Gamer;
            var trumpCard = Card.CleanDeck[2];
            var desk = new List<DeskCardsSlot> { new DeskCardsSlot(null) };
            desk.Clear();
            var game = new GameModel(botHand, gamerHand, deck, desk, trumpCard, whosTurn);

            game.TurnPlayer2Bot(0);
            var canTransfer = game.CanTransferBot();
            game.DoTransferBot();

            Assert.AreEqual(true, canTransfer);
            Assert.AreEqual(2, game.DeskCards.Count);
        }
    }
}

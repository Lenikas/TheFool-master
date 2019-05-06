using System;
using System.Collections.Generic;

namespace Fool
{
    public class Game
    {
        public Game()
        {
            Cards = new Dictionary<Suits, Ranks>();
            CurrentState = new GameState();

            var suitValues = Enum.GetValues(typeof(Suits));
            var rankValues = Enum.GetValues(typeof(Ranks));
            //создание колоды
            for (var i = 0; i < suitValues.Length; i++)
            for (var j = 0; j < rankValues.Length; j++)
            {
                var currentSuit = (Suits) suitValues.GetValue(i);
                var currentRank = (Ranks) rankValues.GetValue(j);
                Cards.Add(currentSuit, currentRank);
            }
        }

        public Dictionary<Suits, Ranks> Cards { get; set; }
        public GameState CurrentState { get; set; }
        public int CountPlayers => CurrentState.Gamers.Count;

        public void DoDistribution()// начало игры, чистим у всех руки от прошлой игры и раздаем карты
        {
            foreach (var gamer in CurrentState.Gamers) gamer.Hand.Clear();
            var indexTrump = CountPlayers * 6 - 1;
            //CurrentState.TrumpSuit = CurrentState.Deck[indexTrump].Suit;
            var cardTrump = CurrentState.Deck[indexTrump];
            CurrentState.Deck.RemoveAt(indexTrump);
            CurrentState.Deck
                .Add(cardTrump); // обработка козыря,раздали карты игрокам,потом взяли следующую и положили под колоду
            // раздаем карты
            for (var i = 0; i < 6; i++)
            for (var j = 0; j < CountPlayers; j++)
            {
                CurrentState.Gamers[j].AddCard(CurrentState.Deck[0]);
                CurrentState.Deck.RemoveAt(0);
            }
        }

        public void DoDistributionInGame() //раздача карты в процессе игры
        {
            for (var i = 0; i < CountPlayers; i++)
            {
                for (var j = 0; j < 6; j++)
                {
                    if (CurrentState.Deck.Count == 0)
                        break;
                    if (CurrentState.Gamers[i].Hand.Count >= 6)
                        continue;
                    CurrentState.Gamers[i].AddCard(CurrentState.Deck[0]);//здесь надо понять,0 индекс в колоде это карта с самого верха или наоборот внизу колоды лежит;
                    CurrentState.Deck.RemoveAt(0);
                }
            }
        }

        //public void ChooseWhoFirst()
        //{
        //    var min = 14;// туз, с енумами не шарю как красиво сделать
        //    foreach (var gamer in CurrentState.Gamers)
        //    {
        //        foreach (var card in gamer.Hand)
        //        {
        //            if (card.Suit == CurrentState.TrumpSuit && card.Rank < min)
        //            {
        //                CurrentState.AttackPlayer = gamer;
        //                var indexAttacker = CurrentState.Gamers.IndexOf(gamer);
        //                var indexDefend = (indexAttacker + 1) % CountPlayers; // это вообще рассчитано на то,что игроков будет больше чем два,
        //               // с двумя может крашнутся, может нет,не думал
        //               CurrentState.DefendPlayer = CurrentState.Gamers[indexDefend];
        //               min = card.Rank;
        //            }

        //        }
        //        // если нету козырей в руках у игроков
        //        if (CurrentState.AttackPlayer == null)
        //        {
        //            CurrentState.AttackPlayer = CurrentState.Gamers[0];
        //            CurrentState.DefendPlayer = CurrentState.Gamers[1];
        //        }

        //    }
        //}

        public void DoStepBot()
        {
            var bot = CurrentState.Gamers[1];
            if (CurrentState.AttackPlayer == bot && bot.Hand != null) //с той мыслью,что игроков всего 2,0 пользователь,1 бот
            {
                CurrentState.Table.Add(bot.Hand[0]);
                bot.RemoveCard(bot.Hand[0]);//тупо выкладываем первую карту из руки
            }
            else
            {
                foreach (var card in bot.Hand)
                {
                    if (card.Suit == CurrentState.Table[0].Suit && card.Rank > CurrentState.Table[0].Rank //||
                        //card.Suit == CurrentState.TrumpSuit && CurrentState.Table[0].Suit != CurrentState.TrumpSuit)
                        )
                    {
                        CurrentState.Table.Add(card);
                        CurrentState.Gamers[1].Hand.Remove(card);
                        CurrentState.BrokenCards.Add(CurrentState.Table[0]);
                        CurrentState.BrokenCards.Add(CurrentState.Table[1]);
                        break;
                    }
                }
            }
        }

        

        public void CheckState()
        {
            var countGamersWithCard = 0;
            foreach (var gamer in CurrentState.Gamers)
                if (gamer.Hand.Count != 0)
                    countGamersWithCard++;

            if (countGamersWithCard <= 1)
                if (countGamersWithCard == 1)
                    CurrentState.EndMessage = "Вы проиграли";
        }
    }
}
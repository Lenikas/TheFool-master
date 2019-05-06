using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fool
{
    public class Card
    {
        public int Rank { get; set; }
        public Suits Suit { get; set; }
        public Bitmap Image;

        public Card Copy()
        {
            return new Card { Rank = this.Rank, Suit = this.Suit, Image = this.Image };
        }

        public static Card[] CleanDeck = new Card[]
        {
            new Card { Rank = 6, Suit = Suits.Spade,    Image = Resource1._1_6 },
            new Card { Rank = 6, Suit = Suits.Heart,    Image = Resource1._2_6 },
            new Card { Rank = 6, Suit = Suits.Diamond,  Image = Resource1._3_6 },
            new Card { Rank = 6, Suit = Suits.Club,     Image = Resource1._4_6 },

            new Card { Rank = 7, Suit = Suits.Spade,    Image = Resource1._1_7 },
            new Card { Rank = 7, Suit = Suits.Heart,    Image = Resource1._2_7 },
            new Card { Rank = 7, Suit = Suits.Diamond,  Image = Resource1._3_7 },
            new Card { Rank = 7, Suit = Suits.Club,     Image = Resource1._4_7 },

            new Card { Rank = 8, Suit = Suits.Spade,    Image = Resource1._1_8 },
            new Card { Rank = 8, Suit = Suits.Heart,    Image = Resource1._2_8 },
            new Card { Rank = 8, Suit = Suits.Diamond,  Image = Resource1._3_8 },
            new Card { Rank = 8, Suit = Suits.Club,     Image = Resource1._4_8 },

            new Card { Rank = 9, Suit = Suits.Spade,    Image = Resource1._1_9 },
            new Card { Rank = 9, Suit = Suits.Heart,    Image = Resource1._2_9 },
            new Card { Rank = 9, Suit = Suits.Diamond,  Image = Resource1._3_9 },
            new Card { Rank = 9, Suit = Suits.Club,     Image = Resource1._4_9 },

            new Card { Rank = 10, Suit = Suits.Spade,   Image = Resource1._1_10 },
            new Card { Rank = 10, Suit = Suits.Heart,   Image = Resource1._2_10 },
            new Card { Rank = 10, Suit = Suits.Diamond, Image = Resource1._3_10 },
            new Card { Rank = 10, Suit = Suits.Club,    Image = Resource1._4_10 },

            new Card { Rank = 11, Suit = Suits.Spade,   Image = Resource1._1_11 },
            new Card { Rank = 11, Suit = Suits.Heart,   Image = Resource1._2_11 },
            new Card { Rank = 11, Suit = Suits.Diamond, Image = Resource1._3_11 },
            new Card { Rank = 11, Suit = Suits.Club,    Image = Resource1._4_11 },

            new Card { Rank = 12, Suit = Suits.Spade,   Image = Resource1._1_12 },
            new Card { Rank = 12, Suit = Suits.Heart,   Image = Resource1._2_12 },
            new Card { Rank = 12, Suit = Suits.Diamond, Image = Resource1._3_12 },
            new Card { Rank = 12, Suit = Suits.Club,    Image = Resource1._4_12 },

            new Card { Rank = 13, Suit = Suits.Spade,   Image = Resource1._1_13 },
            new Card { Rank = 13, Suit = Suits.Heart,   Image = Resource1._2_13 },
            new Card { Rank = 13, Suit = Suits.Diamond, Image = Resource1._3_13 },
            new Card { Rank = 13, Suit = Suits.Club,    Image = Resource1._4_13 },

            new Card { Rank = 14, Suit = Suits.Spade,    Image = Resource1._1_14 },
            new Card { Rank = 14, Suit = Suits.Heart,    Image = Resource1._2_14 },
            new Card { Rank = 14, Suit = Suits.Diamond,  Image = Resource1._3_14 },
            new Card { Rank = 14, Suit = Suits.Club,     Image = Resource1._4_14 },
        };
    }

    public enum Ranks
    {
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14
    }

    public enum Suits
    {
        Spade,      //Пики
        Heart,      //Червы
        Diamond,    //Бубны
        Club,       //Крести
    }
}

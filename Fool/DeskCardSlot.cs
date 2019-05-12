using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fool
{
    public class DeskCardsSlot
    {
        public Card Fore { get; private set; }
        public Card Back { get; private set; }
        public bool ContainsBack { get; private set; }
        public bool ContainsFore { get; private set; }
        public static Size Size = new Size(90, 111);

        public DeskCardsSlot(Card back)
        {
            Back = back;
            ContainsBack = true;
            ContainsFore = false;
        }

        public void Close(Card fore)
        {
            Fore = fore;
            ContainsFore = true;
        }
    }
}

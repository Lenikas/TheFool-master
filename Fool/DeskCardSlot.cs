using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fool
{
    public class DeskCardsSlot
    {
        public Card Fore { get; private set; }
        public Card Back { get; private set; }
        public bool Closed { get; private set; }

        public DeskCardsSlot(Card back)
        {
            Back = back;
        }

        public void Close(Card fore)
        {
            Fore = fore;
            Closed = true;
        }

        public void Clear()
        {
            Fore = null;
            Back = null;
            Closed = false;
        }
    }
}

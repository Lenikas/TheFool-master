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
        public bool ContainsBack { get; private set; }
        public bool ContainsFore { get; private set; }

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

        public void Clear()
        {
            Fore = null;
            Back = null;
            ContainsBack = false;
            ContainsFore = false;
        }
    }
}

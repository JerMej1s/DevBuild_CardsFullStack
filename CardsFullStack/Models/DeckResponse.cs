using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsFullStack.Models
{
    public class DeckResponse
    {
        public string deck_id { get; set; }
        public int remaining { get; set; }
        public IEnumerable<CardResponse> cards { get; set; }
    }
}

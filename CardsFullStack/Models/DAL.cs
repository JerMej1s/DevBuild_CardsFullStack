using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace CardsFullStack.Models
{
    public class DAL
    {
        // ==========================================================================================
        //
        //  THESE ARE CALLED "HIGH LEVEL DATABASE HELPER FUNCTIONS"
        //
        //      These are the only functions where DeckResponse and CardResponse classes are used.
        //      The rest of the app uses Deck and Card classes.
        //
        // ==========================================================================================

        //  Initialize a deck
        //      Get back a deck from the API
        //      Save the deck in database
        //      Draw two cards
        //      Save those cards in database
        //      Return those cards

        // Note: When building an async function, wrap your return type inside Task< .. . >
        public static async Task<IEnumerable<Card>> InitializeDeck()
        {
            // Step 1: Call the API for a new shuffled deck
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://deckofcardsapi.com");
            var response = await client.GetAsync("/api/deck/new/shuffle/?deck_count=1");
            DeckResponse deckResponse = await response.Content.ReadAsAsync<DeckResponse>(); // This needs NuGet package Microsoft.AspNet.WebApi.Client

            // Step 2: Save the deck in the database (using saveDeck() below)
            Deck deck = saveDeck(deckResponse.deck_id, "user100");

            //// Step 3: Call the APIto get two cards for that deck
            //response = await client.GetAsync($"https://deckofcardsapi.com/api/deck/{deck.deck_id}/draw/?count=2");
            //DeckResponse deckResponse2 = await response.Content.ReadAsAsync<DeckResponse>();

            //// Step 4: Save those cards in database (using saveCards() below)
            //foreach (CardResponse cardResponse in deckResponse2.cards)
            //{
            //    saveCard(deck.deck_id, cardResponse.image, cardResponse.code, "user100");
            //}

            //// Step 5: Return that list of Card instances (not a list of CardResponse instances)
            //return getCardsForDeck(deck.deck_id);

            return await DrawTwoCards(deck.deck_id);
        }

        //  Get more cards
        //      Get two cards (from which deck? - this will be a parameter)
        //      Save those cards in database
        //      Return those cards
        public static async Task<IEnumerable<Card>> DrawTwoCards(string deck_id)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://deckofcardsapi.com");
            var response = await client.GetAsync($"/api/deck/{deck_id}/draw/?count=2");
            DeckResponse deckResponse2 = await response.Content.ReadAsAsync<DeckResponse>(); // This needs NuGet package Microsoft.AspNet.WebApi.Client

            // Step 4: Save those cards in database (using saveCards() below)
            foreach (CardResponse cardResponse in deckResponse2.cards)
            {
                saveCard(deck_id, cardResponse.image, cardResponse.code, "user100");
            };

            //// Step 5: Return that list of Card instances (not a list of CardResponse instances)
            return getCardsForDeck(deck_id);
        }

            // ==========================================================================================
            //
            //  THESE ARE CALLED "LOWER LEVEL DATABASE METHODS"
            //
            //      They have no API calls (and therefore no kowledge of the API classes).
            //          That is, the functions below will not use DeckResponse or CardResponse.
            //
            // ==========================================================================================

        public static MySqlConnection Database;
        // Add a new deck to Deck table
        public static Deck saveDeck(string deck_id, string username)
        {
            Deck deck = new Deck()
            {
                deck_id = deck_id,
                username = username,
                created_at = DateTime.Now
            };

            Database.Insert(deck);
            return deck;
        }

        // Get the latest deck from Deck table
        public static Deck getLatestDeck()
        {
            IEnumerable<Deck> result = Database.Query<Deck>("select * from Deck order by created_at desc limit 1"); // This needs using statement for Dapper
            Deck latestDeck = result.First();
            return latestDeck;
        }

        // Save a set of cards to Card table for a particular deck
        public static void saveCards(IEnumerable<Card> cards)
        {
            foreach (Card card in cards)
            {
                Database.Insert(card);
            }
        }

        // Save a single card
        public static Card saveCard(string deck_id, string image, string card_code, string username)
        {
            Card card = new Card()
            {
                deck_id = deck_id,
                image = image,
                card_code = card_code,
                username = username,
                created_at = DateTime.Now
            };

            Database.Insert(card);
            return card;
        }

        // Read all current cards in a particular deck
        public static IEnumerable<Card> getCardsForDeck(string deck_id)
        {
            var parameter = new { deck_id = deck_id };
            IEnumerable<Card> result = Database.Query<Card>("select * from Card where deck_id = @deck_id", parameter);
            return result;
        }
    }
}

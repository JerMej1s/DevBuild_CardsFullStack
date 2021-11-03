using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardsFullStack.Models;

namespace CardsFullStack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        [HttpGet("deck")]
        public async Task<IEnumerable<Card>> GetDeck()
        {
            return await DAL.InitializeDeck();
        }

        [HttpGet("cards/{id}")]
        public async Task<IEnumerable<Card>> GetCards(string deck_id)
        {
            return await DAL.DrawTwoCards(deck_id);
        }

        [HttpGet("test")]
        public async Task<IEnumerable<Card>> runTest()
        {
            return await DAL.InitializeDeck();
        }
    }
}

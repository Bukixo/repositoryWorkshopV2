using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryWorkshopCRUD.Models;
using RepositoryWorkshopCRUD.Repository;

namespace RepositoryWorkshopCRUD.Controllers
{
    [Route("api/burgers")]
    [ApiController]
    public class BurgersController : Controller

    {
        readonly IBurgerRepository burgerRepository;
        public BurgersController(IBurgerRepository burgerRepository)
        {
            this.burgerRepository = burgerRepository;
        }


        // GET: api/Burgers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Burger>>> GetBurger()
        {

            return await burgerRepository.ListAllBurgers();

        }

        // GET: api/Burgers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Burger>> GetBurger(int id)
        {

            var burger = await burgerRepository.GetBurgerByID(id);

            if (burger == null)
            {
                return NotFound();
            }

            return burger;
        }

        // PUT: api/Burgers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBurger(int id, Burger burger)
        {
            {
                if (id != burger.Id)
                {
                    return BadRequest();
                }

                await burgerRepository.UpdateBurger(burger);
                return NoContent();
            }
        }

        // POST: api/Burgers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Burger>> PostBurger(Burger burger)
        {

            await burgerRepository.InsertBurger(burger);
            return CreatedAtAction("GetBurger", new { id = burger.Id }, burger);

        }

        // DELETE: api/Burgers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Burger>> DeleteBurger(int id)
        {
            var burger = await burgerRepository.DeleteBurger(id);
            if (burger == null)
            {
                return NotFound();
            }
            return burger;
        }
    }
}
using Microsoft.EntityFrameworkCore;
using RepositoryWorkshopCRUD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepositoryWorkshopCRUD.Repository
{
    public class BurgerRepository : IBurgerRepository
    {
        private readonly BurgerContext context;


        public BurgerRepository(BurgerContext context)
        {
            this.context = context;
        }

        public void Save()
        {
            context.SaveChanges();
        }


        Task<List<Burger>> IBurgerRepository.ListAllBurgers()
        {
            return context.Burgers.ToListAsync();
        }


        async Task<Burger> IBurgerRepository.GetBurgerByID(int id)
        {
            return await context.Burgers.FindAsync(id);
        }

        async Task<Burger> IBurgerRepository.InsertBurger(Burger burger)
        {
            context.Burgers.Add(burger);
            await context.SaveChangesAsync();
            return burger;
        }

        async Task<Burger> IBurgerRepository.DeleteBurger(long id)
        {
            var burger = await context.Burgers.FindAsync(id);
            if (burger == null)
            {
                return burger;
            }

            context.Burgers.Remove(burger);
            await context.SaveChangesAsync();
            return burger;
        }

        async Task<Burger> IBurgerRepository.UpdateBurger(Burger burger)
        {
            context.Entry(burger).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return burger;

        }

        Task<Burger> IBurgerRepository.Save()
        {
            throw new NotImplementedException();
        }
    }
}

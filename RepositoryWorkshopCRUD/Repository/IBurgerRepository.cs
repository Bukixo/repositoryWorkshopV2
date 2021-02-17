using RepositoryWorkshopCRUD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepositoryWorkshopCRUD.Repository
{
    public interface IBurgerRepository
    {
        Task<List<Burger>> ListAllBurgers();
        Task<Burger> GetBurgerByID(int id);
        Task<Burger> InsertBurger(Burger burger);
        Task<Burger> DeleteBurger(int burgerID);
        Task<Burger> UpdateBurger(Burger burger);
    }
}

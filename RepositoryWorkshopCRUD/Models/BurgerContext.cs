using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepositoryWorkshopCRUD.Models
{
    public class BurgerContext : DbContext
    {
        public BurgerContext(DbContextOptions<BurgerContext> options) : base(options){}
        public DbSet<Burger> Burgers { get; set; }

    }
}

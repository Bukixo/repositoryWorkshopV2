# repositoryWorkshopV2

Here we have a simple crud application that makes a requests to the api that we have defined.
The controller section oof the App defines the call requests that we make. As you can see it is tightly coupled to the the database context. This could cause problems,
one of them being going against the SOLID principles in coding - namely the single responsiblity. We do not want out controller to be involved in querying the databse directly.

Let us start by refactoring and introducing a repository.

## Creating an interface

The first thing we will do is create a new folder called Repository. Next we will define an interface for our repository called IBurgerRepository. That way we can set out a contract to our repository and define exactly which methods will be used in our later BurgerRepository.
```
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
        Task<Burger> Save();
    }
}
```

## Defining the repository

Now that we have our interface, we will create our BurgerRepository, that will inherit from out interface.
It is here that we will essentially take the code from our controller and edit it slightly in order to make our connection to the database context.
That way we can ensure our layer between the repository and the service layer.

```
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

        async Task<Burger> IBurgerRepository.DeleteBurger(int id)
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
    }
}
```

let's break the above code down now line by line!

We firstly, inherit from our interface to get hold of all our methods that will be used.

```
public class BurgerRepository : IBurgerRepository
    {
        private readonly BurgerContext context;


        public BurgerRepository(BurgerContext context)
        {
            this.context = context;
        }
        ////
```

Here we are defining a get task method to essentially to return a list of type Burger.
```
Task<List<Burger>> IBurgerRepository.ListAllBurgers()
{
    return context.Burgers.ToListAsync();
}

```

Next, we use the id that we have captured from the user input to search and find and return a burger that matches the id.
```
async Task<Burger> IBurgerRepository.GetBurgerByID(int id)
{
    return await context.Burgers.FindAsync(id);
}
```

As the name of the Task already suggest, we will be adding a new burger into the database
```
async Task<Burger> IBurgerRepository.InsertBurger(Burger burger)
{
    context.Burgers.Add(burger);
    await context.SaveChangesAsync();
    return burger;
}
```

To delete a burger, we use the the id provided from the user input and make a search. If the burger is non-existent it will return null, if it is found, using entity framework 
it will be removed.
```
async Task<Burger> IBurgerRepository.DeleteBurger(int id)
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
```

Lastly, EntityState allows us to make changes and update we make.
```
async Task<Burger> IBurgerRepository.UpdateBurger(Burger burger)
{
    context.Entry(burger).State = EntityState.Modified;
    await context.SaveChangesAsync();
    return burger;
}
```

### Register the Repository

Next we want to do before adding our repository into the controller, is regstering it into the startup.cs

```
using System.Threading.Tasks;
using RepositoryWorkshopCRUD.Models;
using Microsoft.EntityFrameworkCore;
using RepositoryWorkshopCRUD.Repository;

namespace RepositoryWorkshopCRUD
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers().AddNewtonsoftJson();
            services.AddDbContext<BurgerContext>(opt => opt.UseInMemoryDatabase("Buger"));
            <strong>services.AddScoped<IBurgerRepository, BurgerRepository>();</strong>
        }

```
        
        



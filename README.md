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
            --> services.AddScoped<IBurgerRepository, BurgerRepository>(); <--- added
        }

```
The startup.cs file defines the Startup Class which is triggered when our application launches. The ConfigureServices, as the name says, conifguresservices that will be used in the application.

Here we are rreigstering the the repository service. The Addscoped method means that our service is created once per request. When a new request is made, a new instance of our service is created. We are saying that when we make a call to the Interface IBurgerRepository, use the BurgerRepository.

Update Controller

Finally we update our controller

let do a comparesion

Class Setup

without repository
```
    private readonly BurgerContext _context;
        public BurgersController(BurgerContext context)
        {
            _context = context;
        }
```
with repository

```
    private readonly IBurgerRepository burgerRepository;
        public BurgersController(IBurgerRepository burgerRepository)
        {
            this.burgerRepository = burgerRepository;
        }
```

Now that we have we are using our repository as the middle man, which means now we are calling our context from the repository, there is no longer any need to define it here.
Instead we define our repository in order to access all of it's methods that we have defined.

Get request

Before Repository
```
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Burger>>> GetBurgers()
        {
            //return await _context.Burgers.ToListAsync(); without the DTO model
            return await _context.Burgers
                .Select(burgerInstance => burgerInstance)
                .ToListAsync();
        }
  ```

After Repository
```
[HttpGet]
        public async Task<ActionResult<IEnumerable<Burger>>> GetBurger()
        {

            return await burgerRepository.ListAllBurgers();

        }
 ```
 
 The first thing we notice is how trimmed down our get request is. It no longer directly talks to our context layer. We have lifted all the responsiblity to the repository so now the controller doesn' need to worry about anything but just to make a http request and wait to for our list of burgers to be returned.
 
 
 GET BY ID
 
 Before repository
 ```
         // GET: api/Burgers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Burger>> GetBurger(int id)
        {
            var burger = await _context.Burgers.FindAsync(id);
            if (burger == null)
            {
                return NotFound();
            }

            return burger;
        }
    ```
    
    After Respository
    
   ```
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
      ```
      
      Here, again we are hiding the implementation of the context searching for a burger. Instead all we do l;et our repository search for the burger vbased on the we recieved. To check if burger exsits, we would expect a nonexistent burger to saved as null which returns a not found. Or Altertivelty, If it does exist, we wait for it to be returned.
      
       Put 
       Before repository
       
       ```
               [HttpPut("{id}")]
        public async Task<IActionResult> PutBurger(int id, Burger burger)
        {
            if (id != burger.Id)
            {
                return BadRequest();
            }

            _context.Entry(burger).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BurgerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
```

       
   After Repository
   ```
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
       
```
Post

 Before repository
 
 ```
         [HttpPost]
        public async Task<ActionResult<Burger>> PostBurger(Burger burger)
        {
            _context.Burgers.Add(burger);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBurgers), new { id = burger.Id });
        }
      ````
 
 after
 ```
    [HttpPost]
        public async Task<ActionResult<Burger>> PostBurger(Burger burger)
        {

            await burgerRepository.InsertBurger(burger);
            return CreatedAtAction("GetBurger", new { id = burger.Id }, burger);

        }
      ```

Delete
  Before repository

```
 [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBurger(int id)
        {
            var burger = await _context.Burgers.FindAsync(id);
            if (burger == null)
            {
                return NotFound();
            }

            _context.Burgers.Remove(burger);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BurgerExists(int id)
        {
            return _context.Burgers.Any(e => e.Id == id);
        }
    }
    
    After
    ```
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
    ```


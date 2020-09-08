using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreHooks.Data;
using NetCoreHooks.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreHooks.Services
{
    public class DataGenerationService
    {
        public static void Initialize(IServiceProvider sp)
        {
            /*use the incoming ServiceProvider object to create a RegistrantContext object. Use the
             context to add new registrants to the InMemory database. Those "rows" will be available to 
            subsequent calls from controllers that accept the injected context.*/
            using (var ctx = new RegistrantContext(sp.GetRequiredService<DbContextOptions<RegistrantContext>>()))
            {
                Registrant r1 = new Registrant { Id = 1, FirstName = "Tom", LastName = "Glavine", UserName = "tom.glavine@oktaice.com", SSN = "123-45-6789" };
                Registrant r2 = new Registrant { Id = 2, FirstName = "Greg", LastName = "Maddux", UserName = "greg.maddux@oktaice.com", SSN = "987-65-4321" };
                Registrant r3 = new Registrant { Id = 3, FirstName = "John", LastName = "Smoltz", UserName = "john.smoltz@oktaice.com", SSN = "986-64-2331" };
                Registrant r4 = new Registrant { Id = 4, FirstName = "Liam", LastName = "Morelli", UserName = "liam.morelli@oktaice.com", SSN = "554-55-4444" };
                Registrant r5 = new Registrant { Id = 5, FirstName = "Quinn", LastName = "Morelli", UserName = "quinn.morelli@oktaice.com", SSN = "222-22-2222" };
                ctx.Registrants.Add(r1);
                ctx.Registrants.Add(r2);
                ctx.Registrants.Add(r3);
                ctx.Registrants.Add(r4);
                ctx.Registrants.Add(r5);
                ctx.SaveChanges();
                System.Diagnostics.Debug.WriteLine("registrant count: " + ctx.Registrants.Count());
            }

        }
    }
}

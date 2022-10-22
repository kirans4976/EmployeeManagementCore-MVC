using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        /*DbContextOptions has generic param and we need DbContextOptions to be applied on Appdbcontext
         we must pass options to base DBclass
        We must include Dbset property for every employee type
        To every query and save employee these linq queries will be translated to underlying sql queries
        If i made a mistake of name attribute, for that we have to add another migration/remove migration/update migration to previous state*/

        /* AppDBContextmodelsnapshot.cs will have all changes, migrations.cs will have up(updated chnages) and down(revert changes)
         * EFMigrationHistory table will have all migrations in 1 place
         * 
         */
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            //we have created seed() as an extension() and moved our seed data code to seed()
            modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}

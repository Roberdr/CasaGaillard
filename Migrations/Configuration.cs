namespace CasaGaillard.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<CasaGaillard.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CasaGaillard.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var roles = new[]
            {
                CasaGaillard.Models.AppRoles.Administrador,
                CasaGaillard.Models.AppRoles.Mantenimiento,
                CasaGaillard.Models.AppRoles.Consulta
            };

            foreach (var roleName in roles)
            {
                if (!context.Roles.Any(r => r.Name == roleName))
                {
                    context.Roles.Add(new IdentityRole(roleName));
                }
            }

            context.SaveChanges();
        }
    }
}

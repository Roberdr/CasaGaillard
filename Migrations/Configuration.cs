namespace CasaGaillard.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<CasaGaillard.Models.GaillardEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CasaGaillard.Models.GaillardEntities context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            using (var identityContext = new CasaGaillard.Models.ApplicationDbContext())
            {
                var roles = new[]
                {
                    CasaGaillard.Models.AppRoles.Administrador,
                    CasaGaillard.Models.AppRoles.Mantenimiento,
                    CasaGaillard.Models.AppRoles.Consulta
                };

                foreach (var roleName in roles)
                {
                    if (!identityContext.Roles.Any(r => r.Name == roleName))
                    {
                        identityContext.Roles.Add(new IdentityRole(roleName));
                    }
                }

                identityContext.SaveChanges();
            }
        }
    }
}

using APIEcommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



//Para trabajar con identity ya no se herada de DbContext si no de IdentityDbContext y los usuarios deben de utilizar ApplicationUser, 
//con esto se logra que la migración sea correcta.
public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { 
    
    }


    //Sobreescritura de la creacion del modelo
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }

    //Con la Sobreescritura que se realizo y con esto ya se cuenta con una estructura solida para trabajar con usuarios
    //personalizados usando .net identity
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

}



//

//dotnet ef database update
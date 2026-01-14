using APIEcommerce.Models;
using Microsoft.AspNetCore.Identity;

namespace APIEcommerce.Data {

    public static class DataSeeder {
        
        public static void seedData(ApplicationDbContext appContext) {
            // Seeding de Roles
            if (!appContext.Roles.Any()) {
                appContext.Roles.AddRange(
                  new IdentityRole { Id = "1", Name = "admin", NormalizedName = "ADMIN" },
                  new IdentityRole { Id = "2", Name = "user", NormalizedName = "USER" }
                );
            }
            // Seeding de Categorías
            if (!appContext.Categories.Any()) {
                appContext.Categories.AddRange(
                  new Category { name = "Ropa y accesorios", creationDate = DateTime.Now },
                  new Category { name = "Electrónicos", creationDate = DateTime.Now },
                  new Category { name = "Deportes", creationDate = DateTime.Now },
                  new Category { name = "Hogar", creationDate = DateTime.Now },
                  new Category { name = "Libros", creationDate = DateTime.Now }
                );
            }
            // Seeding de Usuario Administrador
            if (!appContext.ApplicationUsers.Any()) {
                PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();
                ApplicationUser adminUser = new ApplicationUser {
                    Id = "admin-001",
                    UserName = "admin@admin.com",
                    NormalizedUserName = "ADMIN@ADMIN.COM",
                    Email = "admin@admin.com",
                    NormalizedEmail = "ADMIN@ADMIN.COM",
                    EmailConfirmed = true,
                    name = "Administrador"
                };
                adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

                ApplicationUser regularUser = new ApplicationUser {
                    Id = "user-001",
                    UserName = "user@user.com",
                    NormalizedUserName = "USER@USER.COM",
                    Email = "user@user.com",
                    NormalizedEmail = "USER@USER.COM",
                    EmailConfirmed = true,
                    name = "Usuario Regular"
                };
                regularUser.PasswordHash = hasher.HashPassword(regularUser, "User123!");

                appContext.ApplicationUsers.AddRange(adminUser, regularUser);
            }
            // Seeding de UserRoles
            if (!appContext.UserRoles.Any()) {
                appContext.UserRoles.AddRange(
                  new IdentityUserRole<string> { UserId = "admin-001", RoleId = "1" }, // Admin
                  new IdentityUserRole<string> { UserId = "user-001", RoleId = "2" }   // User
                );
            }

            // Seeding de Productos
            if (!appContext.Products.Any()) {
                appContext.Products.AddRange(
                  new Product {
                      name = "Camiseta Básica",
                      description = "Camiseta de algodón 100%",
                      price = 25.99m,
                      sku = "PROD-001-CAM-M",
                      stock = 50,
                      category_id = 1,
                      category = appContext.Categories.Find(1)!,
                      imgUrl = "https://via.placeholder.com/300x300/FF0000/FFFFFF?text=Camiseta",
                      creationDate = DateTime.Now
                  },
                  new Product {
                      name = "Smartphone Galaxy",
                      description = "Teléfono inteligente con 128GB",
                      price = 599.99m,
                      sku = "PROD-002-PHO-BLK",
                      stock = 25,
                      category_id = 2,
                      category = appContext.Categories.Find(2)!,
                      imgUrl = "https://via.placeholder.com/300x300/0000FF/FFFFFF?text=Smartphone",
                      creationDate = DateTime.Now
                  },
                  new Product {
                      name = "Pelota de Fútbol",
                      description = "Pelota oficial FIFA",
                      price = 45.00m,
                      sku = "PROD-003-BAL-WHT",
                      stock = 30,
                      category_id = 3,
                      category = appContext.Categories.Find(3)!,
                      imgUrl = "https://via.placeholder.com/300x300/00FF00/FFFFFF?text=Pelota",
                      creationDate = DateTime.Now
                  },
                  new Product {
                      name = "Lámpara de Mesa",
                      description = "Lámpara LED regulable",
                      price = 89.99m,
                      sku = "PROD-004-LAM-WHT",
                      stock = 15,
                      category_id = 4,
                      category = appContext.Categories.Find(4)!,
                      imgUrl = "https://via.placeholder.com/300x300/FFFF00/000000?text=Lampara",
                      creationDate = DateTime.Now
                  },
                  new Product {
                      name = "El Quijote",
                      description = "Novela clásica de Cervantes",
                      price = 19.99m,
                      sku = "PROD-005-LIB-ESP",
                      stock = 100,
                      category_id = 5,
                      category = appContext.Categories.Find(5)!,
                      imgUrl = "https://via.placeholder.com/300x300/800080/FFFFFF?text=Libro",
                      creationDate = DateTime.Now
                  },
                  new Product {
                      name = "Jeans Clásicos",
                      description = "Pantalones vaqueros azules",
                      price = 79.99m,
                      sku = "PROD-006-PAN-BLU",
                      stock = 40,
                      category_id = 1,
                      category = appContext.Categories.Find(1)!,
                      imgUrl = "https://via.placeholder.com/300x300/4169E1/FFFFFF?text=Jeans",
                      creationDate = DateTime.Now
                  },
                  new Product {
                      name = "Tablet Pro",
                      description = "Tablet 10.5 pulgadas con stylus incluido",
                      price = 459.99m,
                      sku = "PROD-007-TAB-SIL",
                      stock = 20,
                      category_id = 2,
                      category = appContext.Categories.Find(2)!,
                      imgUrl = "https://via.placeholder.com/300x300/C0C0C0/000000?text=Tablet",
                      creationDate = DateTime.Now
                  },
                  new Product {
                      name = "Zapatillas Running",
                      description = "Zapatillas deportivas para correr",
                      price = 129.99m,
                      sku = "PROD-008-ZAP-BLK",
                      stock = 35,
                      category_id = 3,
                      category = appContext.Categories.Find(3)!,
                      imgUrl = "https://via.placeholder.com/300x300/000000/FFFFFF?text=Zapatillas",
                      creationDate = DateTime.Now
                  },
                  new Product {
                      name = "Cafetera Express",
                      description = "Cafetera automática con molinillo integrado",
                      price = 299.99m,
                      sku = "PROD-009-CAF-BLK",
                      stock = 12,
                      category_id = 4,
                      category = appContext.Categories.Find(4)!,
                      imgUrl = "https://via.placeholder.com/300x300/2F4F4F/FFFFFF?text=Cafetera",
                      creationDate = DateTime.Now
                  },
                  new Product {
                      name = "Programación en C#",
                      description = "Guía completa de programación en C# y .NET",
                      price = 49.99m,
                      sku = "PROD-010-LIB-ESP",
                      stock = 80,
                      category_id = 5,
                      category = appContext.Categories.Find(5)!,
                      imgUrl = "https://via.placeholder.com/300x300/008B8B/FFFFFF?text=C%23+Book",
                      creationDate = DateTime.Now
                  },
                  new Product {
                      name = "Chaqueta Deportiva",
                      description = "Chaqueta impermeable para actividades al aire libre",
                      price = 149.99m,
                      sku = "PROD-011-CHA-NAV",
                      stock = 28,
                      category_id = 1,
                      category = appContext.Categories.Find(1)!,
                      imgUrl = "https://via.placeholder.com/300x300/000080/FFFFFF?text=Chaqueta",
                      creationDate = DateTime.Now
                  },
                  new Product {
                      name = "Auriculares Bluetooth",
                      description = "Auriculares inalámbricos con cancelación de ruido",
                      price = 189.99m,
                      sku = "PROD-012-AUR-BLK",
                      stock = 45,
                      category_id = 2,
                      category = appContext.Categories.Find(2)!,
                      imgUrl = "https://via.placeholder.com/300x300/1C1C1C/FFFFFF?text=Auriculares",
                      creationDate = DateTime.Now
                  }
                );
            }
            appContext.SaveChanges();
        }

    }

}

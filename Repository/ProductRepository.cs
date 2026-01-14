using APIEcommerce.Models;
using APIEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace APIEcommerce.Repository {

    public class ProductRepository : IProductRepository {

        private readonly ApplicationDbContext db;

        public ProductRepository(ApplicationDbContext db) {
            this.db = db;
        }

        public bool buyProduct(string name, int quantity) {

            if (string.IsNullOrWhiteSpace(name) || quantity <= 0) return false;

            Product product = this.db.Products.FirstOrDefault(p => p.name.ToLower().Trim() == name.ToLower().Trim());

            if (product == null || product.stock < quantity) return false;

            product.stock -= quantity;

            this.db.Products.Update(product);

            return save();
        }


        public bool createProduct(Product product) {

            if (product == null) return false;

            product.creationDate = DateTime.Now;
            product.updateDate = DateTime.Now;

            this.db.Products.Add(product);

            return save();
        }

        public bool deleteProduct(Product product) {

            if (product == null) return false;

            this.db.Products.Remove(product);

            return save();
        }

        public Product? getProduct(int id) {
            
            if (id <= 0) return null;

            //En la respuesta incluimos la categoria.
            return this.db.Products.Include(p => p.category).FirstOrDefault(p => p.id == id);
        }

        public ICollection<Product> getProducts() {
            return this.db.Products.Include(p => p.category).OrderBy(p => p.name).ToList();
        }

        public ICollection<Product> getProductsByCategory(int categoryId) {

            if (categoryId <= 0) return new List<Product>();

            return this.db.Products.Include(p => p.category).Where(p => p.category_id == categoryId).OrderBy(p => p.name).ToList();
        }

        public ICollection<Product> getProductsInPage(int pageNumber, int pageSize) {
            return this.db.Products.OrderBy(p => p.id)
                .Skip((pageNumber-1) * pageSize).Take(pageSize).ToList(); //Con Skip saltamos el numero de productos que vamos a solicitar, luego una porcion de la lista que nos devuelve
        }

        public int getTotalProducts() {
            return this.db.Products.Count();
        }

        public bool productExists(int id) {

            if (id <= 0) return false;

            return this.db.Products.Any(p => p.id == id);
        }

        public bool productExists(string name) {

            if (string.IsNullOrWhiteSpace(name)) return false;

            return this.db.Products.Any(p => p.name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool save() {
            //return this.db.SaveChanges() > 0 ? true : false;
            return this.db.SaveChanges() > 0;
        }

        public ICollection<Product> searchProducts(string searchTerm) {

            IQueryable<Product> query = this.db.Products;

            string searchTermLowered = searchTerm.ToLower().Trim();

            //Contains busca en toda la cadena si existe el termino que se esta buscando
            if (!string.IsNullOrEmpty(searchTerm)) {
                query = query.Include(p => p.category).Where(
                    p => p.name.ToLower().Trim().Contains(searchTermLowered) ||
                    p.description.ToLower().Trim().Contains(searchTermLowered));
            }

            return query.OrderBy(p => p.name).ToList();
        }

        public bool updateProduct(Product product) {
            if (product == null) return false;

            product.updateDate = DateTime.Now;

            this.db.Products.Update(product);

            return save();
        }
    }

}

//Revisar el siguiente link https://learn.microsoft.com/es-mx/ef/core/querying/related-data/eager que menciona sobre 
//como mejorar los problemas de rendimiento del Include.



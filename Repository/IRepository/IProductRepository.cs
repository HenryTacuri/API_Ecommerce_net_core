using APIEcommerce.Models;

namespace APIEcommerce.Repository.IRepository {

    public interface IProductRepository {

        ICollection<Product> getProducts();

        ICollection<Product> getProductsInPage(int pageNumber, int pageSize);

        int getTotalProducts();

        ICollection<Product> getProductsByCategory(int categoryId);

        ICollection<Product> searchProducts(string searchTerm);

        Product? getProduct(int id);

        bool buyProduct(string name, int quantity);

        bool productExists(int id);

        bool productExists(string name);

        bool createProduct(Product product);

        bool updateProduct(Product product);

        bool deleteProduct(Product product);

        bool save();

    }

}


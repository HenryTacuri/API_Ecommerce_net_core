public interface ICategoryRepository {

    ICollection<Category> getCategories();

    Category? getCategory(int id);

    bool categoryExists(int id);

    bool categoryExists(string name);

    bool createCategory(Category category);

    bool updateCategory(Category category);

    bool deleteCategory(Category category);

    bool save();

}

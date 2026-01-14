
public class CategoryRepository : ICategoryRepository {

    private readonly ApplicationDbContext db;

    public CategoryRepository(ApplicationDbContext db) {
        this.db = db;
    }

    public bool categoryExists(int id) {
        return this.db.Categories.Any(category => category.id == id);
    }

    public bool categoryExists(string name) {
        return this.db.Categories.Any(category => category.name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool createCategory(Category category) {
        category.creationDate = DateTime.Now;
        this.db.Categories.Add(category);
        return save();
    }

    public bool deleteCategory(Category category) {
        this.db.Categories.Remove(category);
        return save();
    }

    public ICollection<Category> getCategories() {
        return this.db.Categories.OrderBy(c => c.name).ToList();
    }

    public Category? getCategory(int id) {
        return this.db.Categories.FirstOrDefault(c => c.id == id);
    }

    public bool save() {

        //Devuelve el numero de escrituras que se han realizado en la base de datos
        return this.db.SaveChanges() > 0 ? true : false;
    }

    public bool updateCategory(Category category) {
        category.creationDate = DateTime.Now;
        this.db.Categories.Update(category);
        return save();
    }

}

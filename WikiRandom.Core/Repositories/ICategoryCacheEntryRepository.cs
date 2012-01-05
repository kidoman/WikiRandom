namespace WikiRandom.Repositories {
    using WikiRandom.DomainObjects;

    public interface ICategoryCacheEntryRepository : IRepository<CategoryCacheEntry> {
        CategoryCacheEntry FindByCategory(Category category);
    }
}

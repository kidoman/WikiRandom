namespace WikiRandom.Strategies {
    using System.Collections.Generic;
    using WikiRandom.DomainObjects;

    public interface IFetchWikiUrlStrategy {
        IEnumerable<CategoryUrl> GetUrl(Category category);
    }
}

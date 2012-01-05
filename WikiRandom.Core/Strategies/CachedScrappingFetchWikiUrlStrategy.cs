namespace WikiRandom.Strategies {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WikiRandom.DomainObjects;
    using WikiRandom.Infrastructure;
    using WikiRandom.Repositories;
    using WikiRandom.Services;

    public class CachedScrappingFetchWikiUrlStrategy : ScrappingFetchWikiUrlStrategy {
        private readonly ICategoryCacheEntryRepository _categoryCacheEntryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CachedScrappingFetchWikiUrlStrategy(IHTMLScrapService htmlScrapService, ICategoryCacheEntryRepository categoryCacheEntryRepository, IUnitOfWork unitOfWork)
            : base(htmlScrapService) {
            _categoryCacheEntryRepository = categoryCacheEntryRepository;
            _unitOfWork = unitOfWork;
        }

        private IEnumerable<CategoryUrl> InternalGetUrl(Category category, CategoryCacheEntry cacheEntry) {
            foreach (var categoryUrl in cacheEntry.CategoryUrls)
                yield return categoryUrl;

            var newUrls = cacheEntry.Invalidate(GetUrlsFromHtmlScrapService(category));

            foreach (var categoryUrl in newUrls)
                yield return categoryUrl;
        }

        public override IEnumerable<CategoryUrl> GetUrl(Category category) {
            var cacheEntry = _categoryCacheEntryRepository.FindByCategory(category);

            if (cacheEntry == null) {
                var categoryUrls = base.GetUrl(category).ToList();

                cacheEntry = new CategoryCacheEntry(_unitOfWork) {
                    Category = category,
                    CachedOn = DateTime.Now,
                    CategoryUrls = categoryUrls
                };
                _categoryCacheEntryRepository.Add(cacheEntry);

                _unitOfWork.Commit();
            }

            return InternalGetUrl(category, cacheEntry);
        }
    }
}

namespace WikiRandom.Strategies {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WikiRandom.DomainObjects;
    using WikiRandom.Services;

    public class ScrappingFetchWikiUrlStrategy : IFetchWikiUrlStrategy {
        private readonly IHTMLScrapService _htmlScrapService;

        public ScrappingFetchWikiUrlStrategy(IHTMLScrapService htmlScrapService) {
            _htmlScrapService = htmlScrapService;
        }

        protected virtual IEnumerable<string> GetUrlsFromHtmlScrapService(Category category) {
            return _htmlScrapService.GetUrls(category.Slug);
        }

        protected internal virtual IEnumerable<CategoryUrl> InternalGetUrl(Category category) {
            var urls = GetUrlsFromHtmlScrapService(category);

            return urls.Select(u => new CategoryUrl { Url = u, DateRetrieved = DateTime.Now });
        }

        public virtual IEnumerable<CategoryUrl> GetUrl(Category category) {
            return InternalGetUrl(category);
        }
    }
}

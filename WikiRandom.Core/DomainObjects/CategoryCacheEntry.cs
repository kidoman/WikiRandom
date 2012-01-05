namespace WikiRandom.DomainObjects {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WikiRandom.Infrastructure;

    public class CategoryCacheEntry : Entity {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryCacheEntry(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public Category Category { get; set; }
        public DateTime CachedOn { get; set; }

        public ICollection<CategoryUrl> CategoryUrls { get; set; }

        public IEnumerable<CategoryUrl> Invalidate(IEnumerable<string> newUrls) {
            var currentMaxDateRetrieved = CategoryUrls.Max(c => c.DateRetrieved);

            foreach (var url in newUrls)
                if (!CategoryUrls.Any(c => c.Url == url))
                    CategoryUrls.Add(new CategoryUrl { Url = url, DateRetrieved = DateTime.Now });

            Save();

            foreach (var categoryUrl in CategoryUrls.Where(c => c.DateRetrieved > currentMaxDateRetrieved))
                yield return categoryUrl;
        }

        public override void Save() {
            _unitOfWork.Commit();
        }
    }
}
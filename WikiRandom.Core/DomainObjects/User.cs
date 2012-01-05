namespace WikiRandom.DomainObjects {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WikiRandom.Strategies;

    public class User : Entity {
        private readonly IFetchWikiUrlStrategy _fetchWikiUrlStrategy;

        public string UserName { get; set; }
        public ICollection<Visit> Visits { get; set; }

        public User(IFetchWikiUrlStrategy fetchWikiUrlStrategy) {
            _fetchWikiUrlStrategy = fetchWikiUrlStrategy;

            Visits = new HashSet<Visit>();
        }

        public CategoryUrl GetNextVisit(Category category) {
            if (category == null)
                throw new ArgumentException(TextMessages.User_CategoryNull, "category");

            foreach (var url in _fetchWikiUrlStrategy.GetUrl(category)) {
                if (!Visits.Any(v => v.CategoryUrl.Url == url.Url)) {
                    var visit = new Visit {
                        CategoryUrl = url,
                        VisitedOn = DateTime.Now
                    };

                    Visits.Add(visit);

                    return url;
                }
            }

            return null;
        }
    }
}

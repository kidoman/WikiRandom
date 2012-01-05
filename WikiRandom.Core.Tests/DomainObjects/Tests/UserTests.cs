using System;
using System.Linq;
using Moq;
using WikiRandom.DomainObjects;
using WikiRandom.Strategies;
using Xunit;

public class UserTests {
    public class TheGetNextVisitMethod {
        [Fact]
        public void ThrowsExceptionWithNullCategory() {
            var user = TestableUser.Create();

            var ex = Assert.Throws<ArgumentException>(() => user.GetNextVisit(null));
            Assert.Equal("category", ex.ParamName);
        }

        [Fact]
        public void WillCallWikiUrlFetchServiceToGetUrlForAGivenCategory() {
            var user = TestableUser.Create();

            var category = new Category {
                Slug = "test"
            };

            var categoryUrl = new CategoryUrl {
                Url = "url",
                DateRetrieved = DateTime.Now
            };

            user.WikiUrlFetchService.Setup(s => s.GetUrl(category))
                                    .Returns(new[] { categoryUrl })
                                    .Verifiable();

            var url = user.GetNextVisit(category);

            user.WikiUrlFetchService.Verify();
        }

        [Fact]
        public void WillReturnNullUrlIfWikiUrlFetchServiceDoesNotReturnAnyUrl() {
            var user = TestableUser.Create();

            var category = new Category {
                Slug = "test"
            };

            user.WikiUrlFetchService.Setup(s => s.GetUrl(category))
                                    .Returns(new CategoryUrl[] { })
                                    .Verifiable();

            var url = user.GetNextVisit(category);

            user.WikiUrlFetchService.Verify();
            Assert.Null(url);
        }

        [Fact]
        public void WillReturnUrlReturnedByWikiUrlFetchServiceIfUrlNotAlreadyVisited() {
            var user = TestableUser.Create();

            var category = new Category {
                Slug = "test"
            };

            var categoryUrl = new CategoryUrl {
                Url = "url",
                DateRetrieved = DateTime.Now
            };

            user.WikiUrlFetchService.Setup(s => s.GetUrl(category))
                                    .Returns(new[] { categoryUrl });

            var url = user.GetNextVisit(category);

            Assert.True(user.Visits.Any(v => v.CategoryUrl == categoryUrl));
            Assert.Same(categoryUrl, url);
        }

        [Fact]
        public void WillNotReturnUrlReturnedByWikiUrlFetchServiceIfUrlAlreadyVisited() {
            var user = TestableUser.Create();

            var category = new Category {
                Slug = "test"
            };

            var categoryUrl = new CategoryUrl {
                Url = "url",
                DateRetrieved = DateTime.Now
            };

            var visit = new Visit {
                CategoryUrl = categoryUrl
            };

            user.WikiUrlFetchService.Setup(s => s.GetUrl(category))
                                    .Returns(new[] { categoryUrl });
            user.Visits.Add(visit);

            var url = user.GetNextVisit(category);

            Assert.Null(url);
        }

        [Fact]
        public void WillNotReturnSameUrlWhenCalledTwiceEvenIfWikiUrlFetchServiceHasOnlyOneUrlAvailable() {
            var user = TestableUser.Create();

            var category = new Category {
                Slug = "test"
            };

            var categoryUrl = new CategoryUrl {
                Url = "url",
                DateRetrieved = DateTime.Now
            };

            user.WikiUrlFetchService.Setup(s => s.GetUrl(category))
                                    .Returns(new[] { categoryUrl });

            var url1 = user.GetNextVisit(category);

            var url2 = user.GetNextVisit(category);

            Assert.NotNull(url1);
            Assert.Null(url2);
        }
    }

    // 1) Should get a unique URL every time it is called (per user.)
    // 2) Should get the list of urls from Wikipedia/Google Search API.
    // 3) Should handle anon users as well.
    // 4) Should have ability to cache the results being fetched from WikiPedia.
}

public class TestableUser : User {
    private readonly Mock<IFetchWikiUrlStrategy> _fetchWikiUrlStrategy;

    public Mock<IFetchWikiUrlStrategy> WikiUrlFetchService {
        get {
            return _fetchWikiUrlStrategy;
        }
    }

    private TestableUser(Mock<IFetchWikiUrlStrategy> fetchWikiUrlStrategy)
        : base(fetchWikiUrlStrategy.Object) {
        _fetchWikiUrlStrategy = fetchWikiUrlStrategy;
    }

    public static TestableUser Create() {
        return new TestableUser(new Mock<IFetchWikiUrlStrategy>());
    }
}






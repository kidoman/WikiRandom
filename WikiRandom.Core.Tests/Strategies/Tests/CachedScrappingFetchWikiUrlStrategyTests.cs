using System.Collections.Generic;
using Moq;
using WikiRandom.DomainObjects;
using WikiRandom.Infrastructure;
using WikiRandom.Repositories;
using WikiRandom.Services;
using WikiRandom.Strategies;
using Xunit;

public class CachedScrappingFetchWikiUrlStrategyTests {
    public class TheGetUrlMethod {
        [Fact]
        public void WillLookupCategoryInTheCategoryCacheEntryRepository() {
            var strategy = TestableCachedScrappingFetchWikiUrlStrategyTests.Create();

            var category = new Category {
                Slug = "test"
            };

            strategy.GetUrl(category);

            strategy.CategoryCacheEntryRepository.Verify(s => s.FindByCategory(category), Times.Once());
        }

        [Fact]
        public void WillCallBaseCallGetUrlIfCacheEntryNotFound() {
            var strategy = TestableCachedScrappingFetchWikiUrlStrategyTests.Create();

            var category = new Category {
                Slug = "test"
            };

            strategy.CategoryCacheEntryRepository.Setup(r => r.FindByCategory(category))
                                                 .Returns((CategoryCacheEntry)null);

            var baseGetUrlCallCount = strategy.BaseClassGetUrlCallCount;

            strategy.GetUrl(category);

            Assert.Equal(baseGetUrlCallCount + 1, strategy.BaseClassGetUrlCallCount);
        }

        [Fact]
        public void WillCreateNewCacheEntryIfEntryNotFoundInRepository() {
            var strategy = TestableCachedScrappingFetchWikiUrlStrategyTests.Create();

            var category = new Category {
                Slug = "test"
            };

            strategy.CategoryCacheEntryRepository.Setup(r => r.FindByCategory(category))
                                                 .Returns((CategoryCacheEntry)null);

            strategy.GetUrl(category);

            strategy.CategoryCacheEntryRepository.Verify(r => r.Add(It.IsAny<CategoryCacheEntry>()), Times.Once());
            strategy.UnitOfWork.Verify(u => u.Commit(), Times.Once());
        }
    }
}

public class TestableCachedScrappingFetchWikiUrlStrategyTests : CachedScrappingFetchWikiUrlStrategy {
    private readonly Mock<IHTMLScrapService> _htmlScrapService;
    private readonly Mock<ICategoryCacheEntryRepository> _categoryCacheEntryRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    public Mock<IHTMLScrapService> HtmlScrapService {
        get {
            return _htmlScrapService;
        }
    }

    public Mock<ICategoryCacheEntryRepository> CategoryCacheEntryRepository {
        get {
            return _categoryCacheEntryRepository;
        }
    }

    public Mock<IUnitOfWork> UnitOfWork {
        get {
            return _unitOfWork;
        }
    }

    public int BaseClassGetUrlCallCount { get; set; }

    private TestableCachedScrappingFetchWikiUrlStrategyTests(Mock<IHTMLScrapService> htmlScrapService,
                                                             Mock<ICategoryCacheEntryRepository> categoryCacheEntryRepository,
                                                             Mock<IUnitOfWork> unitOfWork)
        : base(htmlScrapService.Object, categoryCacheEntryRepository.Object, unitOfWork.Object) {
        _htmlScrapService = htmlScrapService;
        _categoryCacheEntryRepository = categoryCacheEntryRepository;
        _unitOfWork = unitOfWork;
    }

    protected override IEnumerable<CategoryUrl> InternalGetUrl(Category category) {
        BaseClassGetUrlCallCount++;

        return base.InternalGetUrl(category);
    }

    public static TestableCachedScrappingFetchWikiUrlStrategyTests Create() {
        return new TestableCachedScrappingFetchWikiUrlStrategyTests(new Mock<IHTMLScrapService>(),
                                                                    new Mock<ICategoryCacheEntryRepository>(),
                                                                    new Mock<IUnitOfWork>());
    }
}
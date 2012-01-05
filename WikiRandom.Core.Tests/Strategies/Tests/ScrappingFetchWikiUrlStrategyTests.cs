using Moq;
using WikiRandom.DomainObjects;
using WikiRandom.Services;
using WikiRandom.Strategies;
using Xunit;

public class ScrappingFetchWikiUrlStrategyTests {
    public class TheGetUrlMethod {
        [Fact]
        public void WillCallTheHTMLScrapServiceToGetListOfUrls() {
            var htmlScrapService = new Mock<IHTMLScrapService>();
            var strategy = new ScrappingFetchWikiUrlStrategy(htmlScrapService.Object);

            var category = new Category {
                Slug = "test"
            };

            strategy.GetUrl(category);

            htmlScrapService.Verify(s => s.GetUrls(It.IsAny<string>()), Times.Once());
        }
    }
}
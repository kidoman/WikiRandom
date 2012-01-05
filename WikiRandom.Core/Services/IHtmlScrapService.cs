namespace WikiRandom.Services {
    using System.Collections.Generic;

    public interface IHTMLScrapService {
        IEnumerable<string> GetUrls(string html);
    }
}

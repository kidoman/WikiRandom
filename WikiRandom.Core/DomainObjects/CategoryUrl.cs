namespace WikiRandom.DomainObjects {
    using System;
    using System.Text.RegularExpressions;

    public class CategoryUrl : Entity {
        public string Url { get; set; }
        public DateTime DateRetrieved { get; set; }

        public bool IsValid() {
            return Regex.IsMatch(Url, "/* replace with pattern for url */");
        }
    }
}

namespace WikiRandom.DomainObjects {
    using System;

    public class Visit : Entity {
        public User User { get; set; }
        public CategoryUrl CategoryUrl { get; set; }
        public DateTime VisitedOn { get; set; }
    }
}

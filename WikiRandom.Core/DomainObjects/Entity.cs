namespace WikiRandom.DomainObjects {
    using System;

    public abstract class Entity : IEntity {
        public int Id { get; set; }

        public virtual void Save() {
            throw new NotImplementedException();
        }
    }
}

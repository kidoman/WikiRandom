namespace WikiRandom.DomainObjects {
    public interface IEntity {
        int Id { get; }

        void Save();
    }
}

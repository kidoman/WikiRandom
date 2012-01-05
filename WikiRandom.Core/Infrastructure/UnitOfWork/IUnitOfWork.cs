namespace WikiRandom.Infrastructure {
    public interface IUnitOfWork {
        void Commit();
    }
}

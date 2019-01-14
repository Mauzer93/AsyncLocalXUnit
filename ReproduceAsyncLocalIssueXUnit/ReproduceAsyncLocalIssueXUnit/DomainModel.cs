namespace ReproduceAsyncLocalIssueXUnit
{
    public class DomainModel
    {
        public DomainModel(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public void DoSomething()
        {
            DomainEventStore.Raise(new DomainModelEvent(Id));
    
        }
    }
}
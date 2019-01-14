namespace ReproduceAsyncLocalIssueXUnit
{
    public class DomainModel2
    {
        public DomainModel2(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public void DoSomething()
        {
            DomainEventStore.Raise(new DomainModelEvent2(Id));

        }
    }
}
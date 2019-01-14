namespace ReproduceAsyncLocalIssueXUnit
{
    public class DomainModel1
    {
        public DomainModel1(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public void DoSomething()
        {
            DomainEventStore.Raise(new DomainModelEvent1(Id));

        }
    }
}
namespace ReproduceAsyncLocalIssueXUnit
{
    public class DomainModel3
    {
        public DomainModel3(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public void DoSomething()
        {
            DomainEventStore.Raise(new DomainModelEvent3(Id));
        }
    }
}
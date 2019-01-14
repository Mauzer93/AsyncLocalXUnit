using System;

namespace ReproduceAsyncLocalIssueXUnit
{
    public class DomainModelEvent3 : IDomainEvent
    {
        public DomainModelEvent3(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public DateTime CreateDate { get; set; }
    }
}
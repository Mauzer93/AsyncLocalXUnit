using System;

namespace ReproduceAsyncLocalIssueXUnit
{
    public class DomainModelEvent1 : IDomainEvent
    {
        public DomainModelEvent1(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public DateTime CreateDate { get; set; }
    }
}
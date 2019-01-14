using System;

namespace ReproduceAsyncLocalIssueXUnit
{
    public class DomainModelEvent2 : IDomainEvent
    {
        public DomainModelEvent2(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public DateTime CreateDate { get; set; }
    }
}
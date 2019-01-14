using System;

namespace ReproduceAsyncLocalIssueXUnit
{
    public class DomainModelEvent : IDomainEvent
    {
        public DomainModelEvent(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public DateTime CreateDate { get; set; }
    }
}
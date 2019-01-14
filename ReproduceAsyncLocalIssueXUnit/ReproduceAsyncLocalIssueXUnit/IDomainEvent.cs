using System;

namespace ReproduceAsyncLocalIssueXUnit
{
    public interface IDomainEvent
    {
        DateTime CreateDate { get; set; }
    }
}

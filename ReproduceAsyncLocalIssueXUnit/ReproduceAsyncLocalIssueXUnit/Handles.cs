﻿namespace ReproduceAsyncLocalIssueXUnit
{
    public interface Handles<T> where T : IDomainEvent
    {
        void Handle(T args);
    }
}

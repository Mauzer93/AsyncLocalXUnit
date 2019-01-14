using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ReproduceAsyncLocalIssueXUnit
{
    public class XunitTest3b : IClassFixture<SomeDataFixture>
    {
        public XunitTest3b(SomeDataFixture someDataFixture)
        {

        }

        [Fact(DisplayName = "XunitTest3b.Test3")]
        [Trait("Test", "test")]
        public async void Test1()
        {
            // arrange

            var domainModel = new DomainModel2(2);
            var eventHandler = RegisterDomainEventHandler();

            // act
            domainModel.DoSomething();

            DomainEventStore.WaitForAllEvents().Wait();

            // assert
            var loggedEvent = eventHandler.RaisedDomainEvents.Single();
            Assert.Equal(2, loggedEvent.Id);

            Assert.Equal(2, AsyncLocalScopedContainer._asyncLocalDictionary.Value.Count());
        }

        private DomainModelEventHandler2 RegisterDomainEventHandler()
        {
            var container = new AsyncLocalScopedContainer();

            container.Register<SingleInstanceFactory>((Type type) => container.Resolve(type));

            DomainEventStore.InstanceFactory = () => container.Resolve<SingleInstanceFactory>();

            var domainEventHandler = new DomainModelEventHandler2();
            container.Register(domainEventHandler);
            DomainEventStore.RegisterHandler<DomainModelEvent2, DomainModelEventHandler2>();

            return domainEventHandler;
        }

        internal class DomainModelEventHandler2 : Handles<DomainModelEvent2>
        {
            internal List<DomainModelEvent2> RaisedDomainEvents = new List<DomainModelEvent2>();

            public void Handle(DomainModelEvent2 e)
            {
                if (e == null)
                {
                    throw new ArgumentNullException(nameof(e));
                }

                RaisedDomainEvents.Add(e);
            }
        }
    }
}

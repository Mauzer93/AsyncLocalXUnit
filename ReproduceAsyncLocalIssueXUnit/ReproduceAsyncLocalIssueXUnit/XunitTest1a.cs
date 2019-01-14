using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ReproduceAsyncLocalIssueXUnit
{
    public class XunitTest1a : IClassFixture<SomeDataFixture>
    {
        public XunitTest1a(SomeDataFixture someDataFixture)
        {

        }

        [Fact(DisplayName = "XunitTest1a.Test1")]
        [Trait("Test", "test")]
        public async void Test1()
        {
            // arrange

            var domainModel = new DomainModel(2);
            var eventHandler = RegisterDomainEventHandler();

            // act
            domainModel.DoSomething();

            DomainEventStore.WaitForAllEvents().Wait();

            // assert
            var loggedEvent = eventHandler.RaisedDomainEvents.Single();
            Assert.Equal(2, loggedEvent.Id);

            Assert.Equal(2, AsyncLocalScopedContainer._asyncLocalDictionary.Value.Count());
        }

        private DomainModelEventHandler RegisterDomainEventHandler()
        {
            var container = new AsyncLocalScopedContainer();

            container.Register<SingleInstanceFactory>((Type type) => container.Resolve(type));

            DomainEventStore.InstanceFactory = () => container.Resolve<SingleInstanceFactory>();

            var domainEventHandler = new DomainModelEventHandler();
            container.Register(domainEventHandler);
            DomainEventStore.RegisterHandler<DomainModelEvent, DomainModelEventHandler>();

            return domainEventHandler;
        }

        internal class DomainModelEventHandler : Handles<DomainModelEvent>
        {
            internal List<DomainModelEvent> RaisedDomainEvents = new List<DomainModelEvent>();

            public void Handle(DomainModelEvent e)
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

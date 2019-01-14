using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ReproduceAsyncLocalIssueXUnit
{
    public class XunitTest2a : IClassFixture<SomeDataFixture>
    {
        public XunitTest2a(SomeDataFixture someDataFixture)
        {

        }

        [Fact(DisplayName = "XunitTest2a.Test1")]
        [Trait("Test", "test")]
        public async void Test1()
        {
            // arrange

            var domainModel = new DomainModel1(2);
            var eventHandler = RegisterDomainEventHandler();

            // act
            domainModel.DoSomething();

            DomainEventStore.WaitForAllEvents().Wait();

            // assert
            var loggedEvent = eventHandler.RaisedDomainEvents.Single();
            Assert.Equal(2, loggedEvent.Id);

            Assert.Equal(2, AsyncLocalScopedContainer._asyncLocalDictionary.Value.Count());
        }

        private DomainModelEventHandler1 RegisterDomainEventHandler()
        {
            var container = new AsyncLocalScopedContainer();

            container.Register<SingleInstanceFactory>((Type type) => container.Resolve(type));

            DomainEventStore.InstanceFactory = () => container.Resolve<SingleInstanceFactory>();

            var domainEventHandler = new DomainModelEventHandler1();
            container.Register(domainEventHandler);
            DomainEventStore.RegisterHandler<DomainModelEvent1, DomainModelEventHandler1>();

            return domainEventHandler;
        }

        internal class DomainModelEventHandler1 : Handles<DomainModelEvent1>
        {
            internal List<DomainModelEvent1> RaisedDomainEvents = new List<DomainModelEvent1>();

            public void Handle(DomainModelEvent1 e)
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

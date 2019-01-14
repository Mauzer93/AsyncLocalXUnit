using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ReproduceAsyncLocalIssueXUnit
{
    public class XunitTest4a : IClassFixture<SomeDataFixture>
    {
        public XunitTest4a(SomeDataFixture someDataFixture)
        {

        }

        [Fact(DisplayName = "XunitTest4a.Test4")]
        [Trait("Test", "test")]
        public async void Test1()
        {
            // arrange

            var domainModel = new DomainModel3(2);
            var eventHandler = RegisterDomainEventHandler();

            // act
            domainModel.DoSomething();

            DomainEventStore.WaitForAllEvents().Wait();

            // assert
            var loggedEvent = eventHandler.RaisedDomainEvents.Single();
            Assert.Equal(2, loggedEvent.Id);

            Assert.Equal(2, AsyncLocalScopedContainer._asyncLocalDictionary.Value.Count());
        }

        private DomainModelEventHandler3 RegisterDomainEventHandler()
        {
            var container = new AsyncLocalScopedContainer();

            container.Register<SingleInstanceFactory>((Type type) => container.Resolve(type));

            DomainEventStore.InstanceFactory = () => container.Resolve<SingleInstanceFactory>();

            var domainEventHandler = new DomainModelEventHandler3();
            container.Register(domainEventHandler);
            DomainEventStore.RegisterHandler<DomainModelEvent3, DomainModelEventHandler3>();

            return domainEventHandler;
        }

        internal class DomainModelEventHandler3 : Handles<DomainModelEvent3>
        {
            internal List<DomainModelEvent3> RaisedDomainEvents = new List<DomainModelEvent3>();

            public void Handle(DomainModelEvent3 e)
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

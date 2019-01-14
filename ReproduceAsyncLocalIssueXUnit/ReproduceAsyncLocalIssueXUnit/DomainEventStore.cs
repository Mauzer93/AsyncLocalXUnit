using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReproduceAsyncLocalIssueXUnit
{
    public delegate object SingleInstanceFactory(Type serviceType);
    public static class DomainEventStore
    {
        private static readonly object _lockObject = new object();

        private static readonly BlockingCollection<Type> Handlers = new BlockingCollection<Type>();

        private static readonly BlockingCollection<Task> RunningEvents = new BlockingCollection<Task>();

        private static Func<SingleInstanceFactory> _instanceFactory;

        public static Func<SingleInstanceFactory> InstanceFactory
        {
            get => _instanceFactory;
            set
            {
                lock (_lockObject)
                {
                    _instanceFactory = value;
                }
            }
        }

        public static void Raise<T>(T domainEvent)
            where T : IDomainEvent
        {
            if (InstanceFactory == null || InstanceFactory() == null) return;

            var foundHandlers = Handlers
                .Where(handler =>
                {
                    var res = handler.GetInterfaces().Where(x =>
                        x.IsGenericType &&
                        x.GetGenericTypeDefinition() == typeof(Handles<>) &&
                        x.GetGenericArguments()[0].IsAssignableFrom(typeof(T))).ToList();  // find the handler that inherits from handles and where the type or parent type is an idomainevent

                    return res.Any();
                })
                .Select(handler => InstanceFactory()(handler)) //resolve the depencencies of the found handler and make an instance of it.
                .Where(x => x != null)
                .ToList();

            RunningEvents.Add(Task.Run(() =>
            {
                foreach (var handler in foundHandlers)
                {
                    handler.GetType().GetMethod(nameof(Handles<T>.Handle)).Invoke(handler, new object[] { domainEvent }); //invoke the handle method of the found handler
                }
            }).ContinueWith(t => RunningEvents.TryTake(out t)));
        }

        public static void RegisterHandler<TDomainEvent, THandler>()
            where TDomainEvent : IDomainEvent
            where THandler : Handles<TDomainEvent>
        {
            if (!Handlers.Contains(typeof(THandler)))
            {
                Handlers.Add(typeof(THandler));
            }
        }

        public static Task WaitForAllEvents()
        {
            return Task.WhenAll(RunningEvents);
        }
    }
}

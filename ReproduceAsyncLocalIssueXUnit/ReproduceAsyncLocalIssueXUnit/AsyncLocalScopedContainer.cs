using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ReproduceAsyncLocalIssueXUnit
{
    public class AsyncLocalScopedContainer
    {
        private const string ContainerKey = "CallContextScopedContainer";

        public void Register<T>(T instance, Type type = null)
        {
            var concurrentDictionary = GetTestContainer() ?? new ConcurrentDictionary<Type, object>();
            concurrentDictionary.AddOrUpdate(type ?? instance.GetType(), instance, (key, oldValue) => instance);
            PersistTestContainer(concurrentDictionary);
        }

        public T Resolve<T>() where T : class
        {
            var concurrentDictionary = GetTestContainer();
            if (concurrentDictionary == null) return ActivateType(typeof(T)) as T;
            if (concurrentDictionary.TryGetValue(typeof(T), out var instance))
            {
                return instance as T;
            }

            return ActivateType(typeof(T)) as T;
        }

        public object Resolve(Type type)
        {
            if (GetTestContainer() == null)
            {
                var result = ActivateType(type);
                Register(result, type);
            }

            if (GetTestContainer().TryGetValue(type, out var instance))
            {
                return instance;
            }

            var newInstance = ActivateType(type);

            Register(newInstance, type);

            return newInstance;
        }

        private object ActivateType(Type type)
        {
            try
            {
                var instance = Activator.CreateInstance(type);
                return instance;
            }
            catch (MissingMethodException)
            {
                return null;
            }
        }

        public static AsyncLocal<ConcurrentDictionary<Type, object>> _asyncLocalDictionary = new AsyncLocal<ConcurrentDictionary<Type, object>>();

        private void PersistTestContainer(ConcurrentDictionary<Type, object> concurrentDictionary)
        {
            _asyncLocalDictionary.Value = concurrentDictionary;
        }

        private ConcurrentDictionary<Type, object> GetTestContainer()
        {
            return _asyncLocalDictionary.Value;
        }
    }
}

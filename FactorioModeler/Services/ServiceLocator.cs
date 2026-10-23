using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioModeler.Services
{
    public class ServiceLocator
    {
        private static readonly Dictionary<Type, Func<object>> _services = new();

        public static void Register<TInterface>(Func<object> factory)
        {
            _services[typeof(TInterface)] = factory;
        }

        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var factory))
                return factory() as T;
            throw new KeyNotFoundException($"Service {typeof(T).Name} not registered");
        }
    }
}

using System;
using System.Collections.Generic;

namespace Pladi.Core
{
    public interface ILoadable
    {
        // [properties]

        int LoadOrder { get; set; }

        // [methods]

        virtual void Initialize() { }

        // [...]

        private static readonly IDictionary<Type, ILoadable> loadables;

        static ILoadable()
        {
            loadables = new Dictionary<Type, ILoadable>();
        }

        internal static void AddInstance(ILoadable loadable)
            => loadables[loadable.GetType()] = loadable;

        public static void LoadInstances()
        {
            foreach (var (_, instance) in loadables)
            {
                instance.Initialize();
            }
        }

        public static T GetInstance<T>() where T : ILoadable
        {
            loadables.TryGetValue(typeof(T), out var instance);
            return (T)instance;
        }
    }
}
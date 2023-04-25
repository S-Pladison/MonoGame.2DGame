using System.Collections.Generic;
using System.Linq;

namespace Pladi.Core
{
    public interface ILoadable
    {
        int LoadOrder { get; set; }
        void Load() { }

        // ...

        private static readonly HashSet<ILoadable> loadables = new();

        internal static void AddInstance(ILoadable loadable)
            => loadables.Add(loadable);

        public static void LoadInstances()
        {
            foreach (var loadable in loadables)
            {
                loadable.Load();
            }
        }

        public static T GetInstance<T>() where T : ILoadable
            => (T)loadables.FirstOrDefault(x => x is T, null);
    }
}
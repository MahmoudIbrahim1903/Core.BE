using System;
using System.Threading;

namespace Emeint.Core.BE.Infrastructure
{
    /// <summary>
    /// Class that can be derived from to create a singleton.
    /// </summary>
    /// <typeparam name="T">Any type inherited from Singleton&lt;T&gt;></typeparam>
    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static readonly Lazy<T> Lazy = new Lazy<T>(() => (T) Activator.CreateInstance(typeof (T), nonPublic: true),
            LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Lazily created instance property.
        /// </summary>
        public static T Instance { get { return Lazy.Value; } }
    }
}
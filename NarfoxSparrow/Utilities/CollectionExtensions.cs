using System;
using System.Collections.Generic;
using System.Linq;

namespace NarfoxSparrow.Utilities
{
    /// <summary>
    /// This provides extension methods that make
    /// it easy to get random values from a collection
    /// </summary>
    public static class CollectionExtensions
    {
        static Random random = new Random();

        /// <summary>
        /// A Linq-like method for getting a random element from an IEnumerable
        /// </summary>
        /// <typeparam name="T">The element type</typeparam>
        /// <param name="array">The array to fetch an element from</param>
        /// <param name="rand">Optional Random instance, if not provided 
        /// the method will use the RandomService.</param>
        /// <returns>An element of type T or default(T)</returns>
        public static T Random<T>(this IEnumerable<T> enumerable, Random rand = null)
        {
            rand = rand ?? random;
            T o;
            var c = enumerable.Count();
            if (c > 0)
            {
                o = enumerable.ElementAt(rand.Next(0, c));
            }
            else
            {
                o = default(T);
            }
            return o;
        }

        /// <summary>
        /// A Linq-like method for getting a random element from an array
        /// </summary>
        /// <typeparam name="T">The element type</typeparam>
        /// <param name="array">The array to fetch an element from</param>
        /// <param name="rand">Optional Random instance, if not provided 
        /// the method will use a static default instance.</param>
        /// <returns>An element of type T or default(T)</returns>
        public static T Random<T>(this Array array, Random rand = null)
        {
            rand = rand ?? random;
            T o;
            var c = array.Length;
            if (c > 0)
            {
                try
                {
                    o = (T)array.GetValue(rand.Next(0, c));
                }
                catch
                {
                    o = default(T);
                }
            }
            else
            {
                o = default(T);
            }
            return o;
        }
    }
}

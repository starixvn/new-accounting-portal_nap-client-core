namespace Fanex.NAP.Client.Helpers.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtension
    {
        public static IDictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> list)
        {
            return list.ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}
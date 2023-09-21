namespace Fanex.NAP.Client.Helpers.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ObjectExtension
    {
        public static string ToNullOrString(this object o)
        {
            return o?.ToString();
        }

        public static IDictionary<string, object> ToPropertyDictionary(this object obj)
        {
            return obj?.GetType().GetProperties()
                .Select(p => new KeyValuePair<string, object>(p.Name, p.GetValue(obj, null)))
                .ToDictionary();
        }
    }
}
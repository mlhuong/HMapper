using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Extensions
{
    public static class DictionaryExtension
    {
        public static void TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (!dic.ContainsKey(key))
                dic.Add(key, value);
        }
    }
}

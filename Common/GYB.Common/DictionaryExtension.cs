using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GYB.Common
{
    public static class DictionaryExtension
    {
        public static string ConvertToString<T, V>(this Dictionary<T, V> dict,string strKeySplit=":",string strItemSplit=",")
        {
            if (dict == null) return string.Empty;
            return string.Join(strItemSplit, dict.Keys.ToList().Select(t => t + strKeySplit + dict[t]));
        }
    }
}

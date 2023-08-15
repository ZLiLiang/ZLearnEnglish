using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.AnyDBConfigProvider
{
    static class Helper
    {
        /// <summary>
        /// 对实现IDictionary<string,string>的对象进行克隆
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static IDictionary<string, string> Clone(this IDictionary<string, string> dict)
        {
            IDictionary<string, string> newDict = new Dictionary<string, string>();
            foreach (var kv in dict)
            {
                newDict[kv.Key] = kv.Value;
            }
            return newDict;
        }

        /// <summary>
        /// 判断新字典是否有区别于旧字典，如果是的话则返回true
        /// </summary>
        /// <param name="oldDict"></param>
        /// <param name="newDict"></param>
        /// <returns></returns>
        public static bool IsChanged(IDictionary<string, string> oldDict, IDictionary<string, string> newDict)
        {
            if (oldDict.Count != newDict.Count)
            {
                return true;
            }
            foreach (var kv in oldDict)
            {
                var oldKey = kv.Key;
                var oldValue = kv.Value;
                if (!newDict.ContainsKey(oldKey))
                {
                    return true;
                }
                var newValue = newDict[oldKey];
                if (oldValue != newValue)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Z.AnyDBConfigProvider
{
    static class JsonElementExtensions
    {
        /// <summary>
        /// 将json元素以字符串形式返回
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string GetValueForConfig(this JsonElement element)
        {
            if (element.ValueKind==JsonValueKind.String)
            {
                //remove the quotes, "ab"-->ab
                return element.GetString();
            }
            else if (element.ValueKind==JsonValueKind.Null||element.ValueKind==JsonValueKind.Undefined)
            {
                //remove the quotes, "null"-->null
                return null;
            }
            else
            {
                return element.GetRawText();
            }
        }
    }
}

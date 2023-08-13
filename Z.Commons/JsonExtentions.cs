﻿using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Z.Commons.JsonConverters;

namespace Z.Commons
{
    public static class JsonExtentions
    {
        //如果不设置这个，那么"雅思真题"就会保存为"\u96C5\u601D\u771F\u9898"
        public readonly static JavaScriptEncoder Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

        public static JsonSerializerOptions CreateJsonSerializerOptions(bool camelCase = false)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { Encoder = Encoder };
            if (camelCase)
            {
                options.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }
            options.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
            return options;
        }

        public static string ToJsonString(this object value, bool camelCase = false)
        {
            var opt = CreateJsonSerializerOptions(camelCase);
            return JsonSerializer.Serialize(value, value.GetType(), opt);
        }
        public static T? ParseJson<T>(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(T);
            }
            var opt = CreateJsonSerializerOptions();
            return JsonSerializer.Deserialize<T>(value, opt);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Commons
{
    public static class StringExtensions
    {
        /// <summary>
        /// 忽略大小写比较两个字符串
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 截取字符串s1最多前maxLen个字符
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="maxLen"></param>
        /// <returns></returns>
        public static string Cut(this string s1, int maxLen)
        {
            if (s1 == null)
            {
                return String.Empty;
            }
            int len = s1.Length <= maxLen ? s1.Length : maxLen;//不能超过字符串的最大大小
            return s1[0..len];
        }
    }
}
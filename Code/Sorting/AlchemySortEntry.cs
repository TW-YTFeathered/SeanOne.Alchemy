// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System.Collections;

namespace SeanOne.Alchemy.Sorting
{
    internal partial class Sort
    {
        /// <summary>
        /// 排序入口方法，根據傳入的指令對物件進行排序
        /// </summary>
        /// <typeparam name="T"> 物件的型別，實際上未使用，僅用於泛型推斷 </typeparam>
        /// <param name="obj"> 要排序的物件</param>
        /// <param name="ins"> 排序指令字串 </param>
        public static void Entry<T>(T obj, string ins)
        {
            if (string.IsNullOrEmpty(ins))
            {
                // 可以返回或拋出異常
                return;
            }

            if (obj is IList list)
            {
                ListSorter.Sort(list, ins);
            }
            else
            {
                // 預計之後會抱錯
            }
        }
    }
}

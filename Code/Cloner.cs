using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SeanOne.Alchemy
{
    /// <summary>
    /// 提供物件深度複製 (Deep Clone) 的工具類別
    /// 使用反射遞迴複製物件，支援陣列與巢狀物件，並避免循環參考
    /// </summary>
    internal class ReflectionCloner
    {
        /// <summary>
        /// 深度複製物件
        /// </summary>
        /// <typeparam name="T"> 物件的型別，不需特別指定，編譯器會自動推斷 </typeparam>
        /// <param name="obj"> 要複製的物件 </param>
        /// <returns> 複製後的新物件，與原始物件內容相同但為不同參考 </returns>
        public static T DeepClone<T>(T obj)
        {
            return (T)DeepCloneInternal(obj, new Dictionary<object, object>(new ReferenceEqualityComparer()));
        }

        /// <summary>
        /// 深度複製的主體邏輯
        /// </summary>
        /// <param name="obj"> 要複製的物件 </param>
        /// <param name="visited"> 已經複製過的物件，用來避免循環參考 </param>
        /// <returns> 複製後的新物件 </returns>
        private static object DeepCloneInternal(object obj, IDictionary<object, object> visited)
        {
            if (obj == null) return null;

            var type = obj.GetType();

            // 基本型別或不可變型別直接回傳
            if (type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal))
                return obj;

            // 如果已經複製過，避免循環參考
            if (visited.ContainsKey(obj))
                return visited[obj];

            // 陣列處理
            if (type.IsArray)
            {
                var array = (Array)obj;
                var cloned = Array.CreateInstance(type.GetElementType(), array.Length);
                visited[obj] = cloned;
                for (int i = 0; i < array.Length; i++)
                {
                    cloned.SetValue(DeepCloneInternal(array.GetValue(i), visited), i);
                }
                return cloned;
            }

            // 建立新物件
            var clone = Activator.CreateInstance(type);
            visited[obj] = clone;

            // 複製所有欄位 (包含 private)
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var fieldValue = field.GetValue(obj);
                var clonedValue = DeepCloneInternal(fieldValue, visited);
                field.SetValue(clone, clonedValue);
            }

            return clone;
        }

        /// <summary>
        /// 用來避免循環參考的比較器
        /// 使用物件的參考相等性 (ReferenceEquals) 來判斷是否為同一個物件
        /// </summary>
        private class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            /// <summary>
            /// 判斷兩個物件是否為同一個參考
            /// </summary>
            /// <param name="x"> 物件 A </param>
            /// <param name="y"> 物件 B </param>
            /// <returns> 若為同一個參考則回傳 true，否則 false </returns>
            public new bool Equals(object x, object y) => ReferenceEquals(x, y);

            /// <summary>
            /// 取得物件的雜湊碼，基於參考而非值
            /// </summary>
            /// <param name="obj"> 要計算雜湊碼的物件 </param>
            /// <returns> 物件的雜湊碼 </returns>
            public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}

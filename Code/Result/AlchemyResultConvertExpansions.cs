// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SeanOne.Alchemy
{
    public static class AlchemyConverterExpansions
    {
        #region Single value conversion
        public static bool GetBoolean(this AlchemyResult result) =>
            bool.Parse(GetItemString(result.RawSource));

        public static char GetChar(this AlchemyResult result) =>
            char.Parse(GetItemString(result.RawSource));

        public static string GetString(this AlchemyResult result) =>
            GetItemString(result.RawSource);

        public static sbyte GetSByte(this AlchemyResult result) =>
            sbyte.Parse(GetItemString(result.RawSource));

        public static byte GetByte(this AlchemyResult result) =>
            byte.Parse(GetItemString(result.RawSource));

        public static short GetInt16(this AlchemyResult result) =>
            short.Parse(GetItemString(result.RawSource));

        public static ushort GetUInt16(this AlchemyResult result) =>
            ushort.Parse(GetItemString(result.RawSource));

        public static int GetInt32(this AlchemyResult result) =>
            int.Parse(GetItemString(result.RawSource));

        public static uint GetUInt32(this AlchemyResult result) =>
            uint.Parse(GetItemString(result.RawSource));

        public static long GetInt64(this AlchemyResult result) =>
            long.Parse(GetItemString(result.RawSource));

        public static ulong GetUInt64(this AlchemyResult result) =>
            ulong.Parse(GetItemString(result.RawSource));

        public static float GetSingle(this AlchemyResult result) =>
            float.Parse(GetItemString(result.RawSource));

        public static double GetDouble(this AlchemyResult result) =>
            double.Parse(GetItemString(result.RawSource));

        public static decimal GetDecimal(this AlchemyResult result) =>
            decimal.Parse(GetItemString(result.RawSource));

        public static DateTime GetDateTime(this AlchemyResult result) =>
            DateTime.Parse(GetItemString(result.RawSource));

        public static Guid GetGuid(this AlchemyResult result) =>
            Guid.Parse(GetItemString(result.RawSource));

        public static TimeSpan GetTimeSpan(this AlchemyResult result) =>
            TimeSpan.Parse(GetItemString(result.RawSource));
        #endregion

        #region List transformation (with default converters)
        // 原有便捷方法: 內部呼叫 GetList 並傳入預設轉換器
        public static List<bool> GetBooleanList(this AlchemyResult result) =>
            GetList(result, bool.Parse);

        public static List<char> GetCharList(this AlchemyResult result) =>
            GetList(result, char.Parse);

        public static List<string> GetStringList(this AlchemyResult result) =>
            GetList(result, GetItemString);

        public static List<sbyte> GetSByteList(this AlchemyResult result) =>
            GetList(result, sbyte.Parse);

        public static List<byte> GetByteList(this AlchemyResult result) =>
            GetList(result, byte.Parse);

        public static List<short> GetInt16List(this AlchemyResult result) =>
            GetList(result, short.Parse);

        public static List<ushort> GetUInt16List(this AlchemyResult result) =>
            GetList(result, ushort.Parse);

        public static List<int> GetInt32List(this AlchemyResult result) =>
            GetList(result, int.Parse);

        public static List<uint> GetUInt32List(this AlchemyResult result) =>
            GetList(result, uint.Parse);

        public static List<long> GetInt64List(this AlchemyResult result) =>
            GetList(result, long.Parse);

        public static List<ulong> GetUInt64List(this AlchemyResult result) =>
            GetList(result, ulong.Parse);

        public static List<float> GetSingleList(this AlchemyResult result) =>
            GetList(result, float.Parse);

        public static List<double> GetDoubleList(this AlchemyResult result) =>
            GetList(result, double.Parse);

        public static List<decimal> GetDecimalList(this AlchemyResult result) =>
            GetList(result, decimal.Parse);

        public static List<DateTime> GetDateTimeList(this AlchemyResult result) =>
            GetList(result, DateTime.Parse);

        public static List<Guid> GetGuidList(this AlchemyResult result) =>
            GetList(result, Guid.Parse);

        public static List<TimeSpan> GetTimeSpanList(this AlchemyResult result) =>
            GetList(result, TimeSpan.Parse);

        // GetObjects 特殊處理 (保留原有邏輯)
        public static List<object> GetObjectList(this AlchemyResult result)
        {
            if (result.RawSource is IEnumerable<object> objects)
                return objects.ToList();

            if (result.RawSource is IEnumerable nonGenericEnumerable)
            {
                var list = new List<object>();
                foreach (var item in nonGenericEnumerable)
                    list.Add(item);
                return list;
            }

            throw new InvalidCastException($"Expected IEnumerable, but got {result.RawSource?.GetType()}");
        }
        #endregion

        #region Private Method
        /// <summary>
        /// 將結果中的集合轉換為 List，使用自訂的轉換器。 
        /// </summary>
        /// <typeparam name="T">目標元素類型</typeparam>
        /// <param name="result">AlchemyResult 實例</param>
        /// <param name="converter">轉換委託，接收元素物件和索引，傳回轉換後的 T</param>
        /// <returns>轉換後的 List</returns>
        private static List<T> GetList<T>(this AlchemyResult result, Func<string, T> converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            // 如果已經是 IEnumerable<T>，直接轉換 (此時元素型別完全符合)
            if (result.RawSource is IEnumerable<T> typedEnumerable)
                return typedEnumerable.ToList();

            // 嘗試以非泛型 IEnumerable 處理
            if (result.RawSource is IEnumerable objects)
            {
                // 嘗試取得容量 (如果支援 ICollection)
                int capacity = 0;
                if (objects is ICollection col)
                    capacity = col.Count;

                var results = capacity > 0 ? new List<T>(capacity) : new List<T>();
                int index = 0;

                foreach (var item in objects)
                {
                    try
                    {
                        results.Add(converter(GetItemString(item, index)));
                    }
                    catch
                    {
                        ThrowConversionFailed(item, index);
                    }

                    index++;
                }

                return results;
            }

            throw new InvalidCastException($"Cannot convert {result.RawSource?.GetType()} to any enumerable type.");
        }

        private static string GetItemString(object item)
        {
            if (item == null)
                throw new InvalidCastException("Item is null and cannot be converted.");
            return item.ToString();
        }
        private static string GetItemString(object item, int index)
        {
            if (item == null)
                throw new InvalidCastException($"Item at index {index} is null and cannot be converted.");
            return item.ToString();
        }

        private static void ThrowConversionFailed(object item, int idx) =>
            throw new InvalidCastException($"Failed to convert item at index {idx}: '{item}'");
        #endregion
    }
}

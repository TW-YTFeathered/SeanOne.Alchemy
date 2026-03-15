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
        public static bool GetBool(this AlchemyResult result) =>
            Convert.ToBoolean(result._source);

        public static char GetChar(this AlchemyResult result) =>
            Convert.ToChar(result._source);

        public static string GetString(this AlchemyResult result) =>
            Convert.ToString(result._source);

        public static sbyte GetSByte(this AlchemyResult result) =>
            Convert.ToSByte(result._source);

        public static byte GetByte(this AlchemyResult result) =>
            Convert.ToByte(result._source);

        public static short GetShort(this AlchemyResult result) =>
            Convert.ToInt16(result._source);

        public static ushort GetUShort(this AlchemyResult result) =>
            Convert.ToUInt16(result._source);

        public static int GetInt(this AlchemyResult result) =>
            Convert.ToInt32(result._source);

        public static uint GetUInt(this AlchemyResult result) =>
            Convert.ToUInt32(result._source);

        public static long GetLong(this AlchemyResult result) =>
            Convert.ToInt64(result._source);

        public static ulong GetULong(this AlchemyResult result) =>
            Convert.ToUInt64(result._source);

        public static float GetFloat(this AlchemyResult result) =>
            Convert.ToSingle(result._source);

        public static double GetDouble(this AlchemyResult result) =>
            Convert.ToDouble(result._source);

        public static decimal GetDecimal(this AlchemyResult result) =>
            Convert.ToDecimal(result._source);

        public static DateTime GetDateTime(this AlchemyResult result) =>
            Convert.ToDateTime(result._source);

        public static Guid GetGuid(this AlchemyResult result) =>
            Guid.Parse(result._source.ToString());

        public static TimeSpan GetTimeSpan(this AlchemyResult result) =>
            TimeSpan.Parse(result._source.ToString());
        #endregion

        #region List transformation (with default converters)
        // 原有便捷方法: 內部呼叫 GetList 並傳入預設轉換器
        public static List<bool> GetBools(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!bool.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<char> GetChars(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!char.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<string> GetStrings(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (item == null)
                    ThrowConversionFailed(item, idx);
                return item.ToString();
            });

        public static List<sbyte> GetSbytes(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!sbyte.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<byte> GetBytes(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!byte.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<short> GetShorts(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!short.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<ushort> GetUShorts(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!ushort.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<int> GetInts(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!int.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<uint> GetUInts(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!uint.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<long> GetLongs(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!long.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<ulong> GetULongs(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!ulong.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<float> GetFloats(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!float.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<double> GetDoubles(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!double.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<decimal> GetDecimals(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!decimal.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<DateTime> GetDateTimes(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!DateTime.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<Guid> GetGuids(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!Guid.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        public static List<TimeSpan> GetTimeSpans(this AlchemyResult result) =>
            GetList(result, (item, idx) =>
            {
                if (!TimeSpan.TryParse(item?.ToString(), out var val))
                    ThrowConversionFailed(item, idx);
                return val;
            });

        // GetObjects 特殊處理 (保留原有邏輯)
        public static List<object> GetObjects(this AlchemyResult result)
        {
            if (result._source is IEnumerable<object> objects)
                return objects.ToList();

            if (result._source is IEnumerable nonGenericEnumerable)
            {
                var list = new List<object>();
                foreach (var item in nonGenericEnumerable)
                    list.Add(item);
                return list;
            }

            throw new InvalidCastException($"Expected IEnumerable, but got {result._source?.GetType()}");
        }
        #endregion

        #region Private generic method with custom converter
        /// <summary>
        /// 將結果中的集合轉換為 List，使用自訂的轉換器。 
        /// </summary>
        /// <typeparam name="T">目標元素類型</typeparam>
        /// <param name="result">AlchemyResult 實例</param>
        /// <param name="converter">轉換委託，接收元素物件和索引，傳回轉換後的 T</param>
        /// <returns>轉換後的 List</returns>
        private static List<T> GetList<T>(this AlchemyResult result, Func<object, int, T> converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            // 如果已經是 IEnumerable<T>，直接轉換 (此時元素型別完全符合)
            if (result._source is IEnumerable<T> typedEnumerable)
                return typedEnumerable.ToList();

            // 嘗試以 IEnumerable<object> 處理 (適用於元素類型為參考類型或已裝箱的情況)
            if (result._source is IEnumerable<object> objects)
            {
                int capacity = objects is ICollection<object> col1 ? col1.Count :
                               objects is ICollection col2 ? col2.Count : 0;
                var results = capacity > 0 ? new List<T>(capacity) : new List<T>();
                int index = 0;
                foreach (var item in objects)
                {
                    results.Add(converter(item, index));
                    index++;
                }
                return results;
            }

            // 嘗試以非泛型 IEnumerable 處理 (如 ArrayList，以及元素為值型別的泛型集合如 List<double> 等)
            if (result._source is IEnumerable nonGeneric)
            {
                // 嘗試取得容量 (如果支援 ICollection)
                int capacity = 0;
                if (nonGeneric is ICollection col)
                    capacity = col.Count;

                var results = capacity > 0 ? new List<T>(capacity) : new List<T>();
                int index = 0;
                foreach (var item in nonGeneric)
                {
                    results.Add(converter(item, index));
                    index++;
                }
                return results;
            }

            throw new InvalidCastException($"Cannot convert {result._source?.GetType()} to any enumerable type.");
        }
        #endregion

        #region Private throw method
        private static void ThrowConversionFailed(object item, int idx) =>
            throw new InvalidCastException($"Failed to convert item at index {idx}: '{item}'");
        #endregion
    }
}

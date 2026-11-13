// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SeanOne.Alchemy;

public static class AlchemyConverterExpansions
{
    extension(AlchemyResult result)
    {
        private string RawStr => GetItemString(result.RawSource);

        public bool GetBoolean() => bool.Parse(result.RawStr);
        public char GetChar() => char.Parse(result.RawStr);
        public string GetString() => result.RawStr;
        public sbyte GetSByte() => sbyte.Parse(result.RawStr);
        public byte GetByte() => byte.Parse(result.RawStr);
        public short GetInt16() => short.Parse(result.RawStr);
        public ushort GetUInt16() => ushort.Parse(result.RawStr);
        public int GetInt32() => int.Parse(result.RawStr);
        public uint GetUInt32() => uint.Parse(result.RawStr);
        public long GetInt64() => long.Parse(result.RawStr);
        public ulong GetUInt64() => ulong.Parse(result.RawStr);
        public float GetSingle() => float.Parse(result.RawStr);
        public double GetDouble() => double.Parse(result.RawStr);
        public decimal GetDecimal() => decimal.Parse(result.RawStr);
        public DateTime GetDateTime() => DateTime.Parse(result.RawStr);
        public Guid GetGuid() => Guid.Parse(result.RawStr);
        public TimeSpan GetTimeSpan() => TimeSpan.Parse(result.RawStr);

        // 原有便捷方法: 內部呼叫 GetList 並傳入預設轉換器
        public List<bool> GetBooleanList() => result.GetList(bool.Parse);
        public List<char> GetCharList() => result.GetList(char.Parse);
        public List<string> GetStringList() => result.GetList(GetItemString);
        public List<sbyte> GetSByteList() => result.GetList(sbyte.Parse);
        public List<byte> GetByteList() => result.GetList(byte.Parse);
        public List<short> GetInt16List() => result.GetList(short.Parse);
        public List<ushort> GetUInt16List() => result.GetList(ushort.Parse);
        public List<int> GetInt32List() => result.GetList(int.Parse);
        public List<uint> GetUInt32List() => result.GetList(uint.Parse);
        public List<long> GetInt64List() => result.GetList(long.Parse);
        public List<ulong> GetUInt64List() => result.GetList(ulong.Parse);
        public List<float> GetSingleList() => result.GetList(float.Parse);
        public List<double> GetDoubleList() => result.GetList(double.Parse);
        public List<decimal> GetDecimalList() => result.GetList(decimal.Parse);
        public List<DateTime> GetDateTimeList() => result.GetList(DateTime.Parse);
        public List<Guid> GetGuidList() => result.GetList(Guid.Parse);
        public List<TimeSpan> GetTimeSpanList() => result.GetList(TimeSpan.Parse);

        // GetObjects 特殊處理 (保留原有邏輯)
        public List<object> GetObjectList() => result.RawSource switch
        {
            IEnumerable<object> objects => objects.ToList(),
            IEnumerable nonGenericEnumerable => [.. nonGenericEnumerable],
            _ => throw new InvalidCastException($"Expected IEnumerable, but got {result.RawSource?.GetType()?.Name}")
        };

        /// <summary>
        /// 將結果中的集合轉換為 List，使用自訂的轉換器。 
        /// </summary>
        /// <typeparam name="T">目標元素類型</typeparam>
        /// <param name="converter">轉換委託，接收元素物件和索引，傳回轉換後的 T</param>
        /// <returns>轉換後的 List</returns>
        private List<T> GetList<T>(Func<string, T> converter)
        {
            if (converter is null)
                throw new ArgumentNullException(nameof(converter));

            // 如果已經是 IEnumerable<T>，直接轉換 (此時元素型別完全符合)
            if (result.RawSource is IEnumerable<T> typedEnumerable)
                return typedEnumerable.ToList();

            // 嘗試以非泛型 IEnumerable 處理
            if (result.RawSource is IEnumerable objects)
            {
                // 嘗試取得容量 (如果支援 ICollection)
                int capacity = objects is ICollection col ? col.Count : 0;
                List<T> results = new(capacity);
                int index = 0;

                foreach (var item in objects)
                {
                    try
                    {
                        results.Add(converter(GetItemString(item, index)));
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidCastException($"Failed to convert item at index {index}: '{item}'", ex);
                    }

                    index++;
                }

                return results;
            }

            throw new InvalidCastException($"Cannot convert {result.RawSource?.GetType()} to any enumerable type.");
        }
    }

    #region Private Method
    private static string GetItemString(object item) => item switch
    {
        null => throw new InvalidCastException("Item is null and cannot be converted."),
        _ => item.ToString()
    };

    private static string GetItemString(object item, int index) => item switch
    {
        null => throw new InvalidCastException($"Item at index {index} is null and cannot be converted."),
        _ => item.ToString()
    };
    #endregion
}

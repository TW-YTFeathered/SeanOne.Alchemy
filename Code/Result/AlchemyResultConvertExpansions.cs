// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SeanOne.Alchemy
{
    public partial class AlchemyResult
    {
        public bool GetBool() => System.Convert.ToBoolean(_source);
        public char GetChar() => System.Convert.ToChar(_source);
        public string GetString() => System.Convert.ToString(_source);
        public sbyte GetSByte() => System.Convert.ToSByte(_source);
        public byte GetByte() => System.Convert.ToByte(_source);
        public short GetShort() => System.Convert.ToInt16(_source);
        public ushort GetUShort() => System.Convert.ToUInt16(_source);
        public int GetInt() => System.Convert.ToInt32(_source);
        public uint GetUInt() => System.Convert.ToUInt32(_source);
        public long GetLong() => System.Convert.ToInt64(_source);
        public ulong GetULong() => System.Convert.ToUInt64(_source);
        public float GetFloat() => System.Convert.ToSingle(_source);
        public double GetDouble() => System.Convert.ToDouble(_source);
        public decimal GetDecimal() => System.Convert.ToDecimal(_source);
        public DateTime GetDateTime() => System.Convert.ToDateTime(_source);
        public Guid GetGuid() => Guid.Parse(_source.ToString());
        public TimeSpan GetTimeSpan() => TimeSpan.Parse(_source.ToString());

        #region Lists
        public List<bool> GetBools()
        {
            if (_source is IEnumerable<bool> bools)
                return bools.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to bool at index {index}");
                if (bool.TryParse(item.ToString(), out bool result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to bool at index {index}");
            });
        }
        public List<char> GetChars()
        {
            if (_source is IEnumerable<char> chars)
                return chars.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to char at index {index}");
                if (char.TryParse(item.ToString(), out char result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to char at index {index}");
            });
        }
        public List<string> GetStrings()
        {
            if (_source is IEnumerable<string> strings)
                return strings.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                return item?.ToString() ?? throw new InvalidCastException($"Cannot convert null to string at index {index}");
            });
        }
        public List<sbyte> GetSbytes()
        {
            if (_source is IEnumerable<sbyte> sbytes)
                return sbytes.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to sbyte at index {index}");
                if (sbyte.TryParse(item.ToString(), out sbyte result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to sbyte at index {index}");
            });
        }
        public List<byte> GetBytes()
        {
            if (_source is IEnumerable<byte> bytes)
                return bytes.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to byte at index {index}");
                if (byte.TryParse(item.ToString(), out byte result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to byte at index {index}");
            });
        }
        public List<short> GetShorts()
        {
            if (_source is IEnumerable<short> shorts)
                return shorts.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to short at index {index}");
                if (short.TryParse(item.ToString(), out short result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to short at index {index}");
            });
        }
        public List<ushort> GetUShorts()
        {
            if (_source is IEnumerable<ushort> ushorts)
                return ushorts.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to ushort at index {index}");
                if (ushort.TryParse(item.ToString(), out ushort result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to ushort at index {index}");
            });
        }
        public List<int> GetInts()
        {
            if (_source is IEnumerable<int> ints)
                return ints.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to int at index {index}");
                if (int.TryParse(item.ToString(), out int result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to int at index {index}");
            });
        }
        public List<uint> GetUInts()
        {
            if (_source is IEnumerable<uint> uints)
                return uints.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to uint at index {index}");
                if (uint.TryParse(item.ToString(), out uint result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to uint at index {index}");
            });
        }
        public List<long> GetLongs()
        {
            if (_source is IEnumerable<long> longs)
                return longs.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to long at index {index}");
                if (long.TryParse(item.ToString(), out long result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to long at index {index}");
            });
        }
        public List<ulong> GetULongs()
        {
            if (_source is IEnumerable<ulong> ulongs)
                return ulongs.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to ulong at index {index}");
                if (ulong.TryParse(item.ToString(), out ulong result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to ulong at index {index}");
            });
        }
        public List<float> GetFloats()
        {
            if (_source is IEnumerable<float> floats)
                return floats.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to float at index {index}");
                if (float.TryParse(item.ToString(), out float result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to float at index {index}");
            });
        }
        public List<double> GetDoubles()
        {
            if (_source is IEnumerable<double> doubles)
                return doubles.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to double at index {index}");
                if (double.TryParse(item.ToString(), out double result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to double at index {index}");
            });
        }
        public List<decimal> GetDecimals()
        {
            if (_source is IEnumerable<decimal> decimals)
                return decimals.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to decimal at index {index}");
                if (decimal.TryParse(item.ToString(), out decimal result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to decimal at index {index}");
            });
        }
        public List<DateTime> GetDateTimes()
        {
            if (_source is IEnumerable<DateTime> dateTimes)
                return dateTimes.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to DateTime at index {index}");
                if (DateTime.TryParse(item.ToString(), out DateTime result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to DateTime at index {index}");
            });
        }
        public List<Guid> GetGuids()
        {
            if (_source is IEnumerable<Guid> guids)
                return guids.ToList();
            return ConvertEnumerableToLinearList((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to Guid at index {index}");
                if (Guid.TryParse(item.ToString(), out Guid result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to Guid at index {index}");
            });
        }
        public List<TimeSpan> GetTimeSpans()
        {
            if (_source is IEnumerable<TimeSpan> timeSpans)
                return timeSpans.ToList();
            return ConvertEnumerableToLinearList<TimeSpan>((item, index) =>
            {
                if (item == null)
                    throw new InvalidCastException($"Cannot convert null to TimeSpan at index {index}");
                if (TimeSpan.TryParse(item.ToString(), out TimeSpan result))
                    return result;
                throw new InvalidCastException($"Cannot convert item '{item}' to TimeSpan at index {index}");
            });
        }

        // GetObjects 的特殊處理 - 允許非泛型集合轉換為 List<object>
        public List<object> GetObjects()
        {
            // 如果已經是 IEnumerable<object>，直接轉換
            if (_source is IEnumerable<object> objects)
                return objects.ToList();

            // 如果是非泛型集合，嘗試轉換
            if (_source is System.Collections.IEnumerable nonGenericEnumerable)
            {
                var result = new List<object>();
                foreach (var item in nonGenericEnumerable)
                    result.Add(item);
                return result;
            }

            throw new InvalidCastException($"Expected IEnumerable<object> or non-generic IEnumerable, but got {_source?.GetType()}");
        }

        // 輔助方法：將 IEnumerable<object> 轉換為 List<T>
        private List<T> ConvertEnumerableToLinearList<T>(Func<object, int, T> converter)
        {
            if (_source is IEnumerable<object> objects)
            {
                // 嘗試取得元素個數 (若支援)
                int capacity = 0;
                if (objects is ICollection<object> col1)
                    capacity = col1.Count;
                else if (objects is ICollection col2)
                    capacity = col2.Count;

                var results = capacity > 0 ? new List<T>(capacity) : new List<T>();
                int index = 0;

                foreach (var item in objects)
                {
                    results.Add(converter(item, index));
                    index++;
                }
                return results;
            }

            throw new InvalidCastException(
                $"Cannot convert type {_source?.GetType()} to List<{typeof(T).Name}>. " +
                $"Expected IEnumerable<{typeof(T).Name}> or IEnumerable<object> with convertible values.");
        }
        #endregion
    }
}

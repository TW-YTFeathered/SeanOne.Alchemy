// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
#if !NET5_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace SeanOne.Alchemy.Utility;

/// <summary>
/// 提供物件深度複製 (Deep Clone) 的工具類別
/// 使用反射遞迴複製物件，支援陣列與巢狀物件，並避免循環參考
/// </summary>
internal class ReflectionCloner
{
    // 循環參考比較器 (單例，省下每次 new 的分配)
    private static readonly IEqualityComparer<object> _refComparer = new ReferenceEqualityComparer();

    // 內建不可變物件
    private static readonly HashSet<Type> _immutableTypes = [
        typeof(Guid), typeof(TimeSpan), typeof(DateTimeOffset), typeof(IntPtr), typeof(UIntPtr), 
        typeof(Version), typeof(Uri), typeof(BigInteger)
    ];

    /// <summary>
    /// 判斷型別是否為不可變型別
    /// </summary>
    public static bool IsImmutableType(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        // 快路徑: TypeCode 非 Object
        // 涵蓋 primitive / enum / string / decimal / DateTime / DBNull
        if (Type.GetTypeCode(type) != TypeCode.Object)
            return true;

        return _immutableTypes.Contains(type) || typeof(Type).IsAssignableFrom(type);
    }

    /// <summary>
    /// 深度複製物件
    /// </summary>
    /// <typeparam name="T"> 物件的型別，不需特別指定，編譯器會自動推斷 </typeparam>
    /// <param name="obj"> 要複製的物件 </param>
    /// <returns> 複製後的新物件，與原始物件內容相同但為不同參考 </returns>
    public static T DeepClone<T>(T obj) =>
        (T)DeepCloneInternal(obj, new Dictionary<object, object>(_refComparer));

    /// <summary>
    /// 深度複製的主體邏輯
    /// </summary>
    /// <param name="obj"> 要複製的物件 </param>
    /// <param name="visited"> 已經複製過的物件，用來避免循環參考 </param>
    /// <returns> 複製後的新物件 </returns>
    private static object DeepCloneInternal(object obj, IDictionary<object, object> visited)
    {
        if (obj is null) return null;

        var type = obj.GetType();

        // 快路徑，避免將不可變型別進入遞迴
        if (IsImmutableType(type))
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
                cloned.SetValue(DeepCloneInternal(array.GetValue(i), visited), i);
            return cloned;
        }

        // 建立新物件
        var clone = CreateInstance(type);
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
    /// 建立型別實例。
    /// 值型別：透過 Activator 建立 (使用預設建構子／零初始化)
    /// 參考型別：若存在無參數建構子（public 或 non-public）則呼叫它；
    /// 否則建立未初始化物件 (不執行任何建構子與欄位初始化器)
    /// </summary>
    private static object CreateInstance(Type type)
    {
        // 值型別直接用 Activator 即可
        if (type.IsValueType)
            return Activator.CreateInstance(type);

        // 優先找無參數建構子 (public / non-public)
        var ctor = type.GetConstructor(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            Type.EmptyTypes,
            null
        );

        if (ctor is not null)
            return ctor.Invoke(null);

#if NET5_0_OR_GREATER
        return RuntimeHelpers.GetUninitializedObject(type);
#else
        return FormatterServices.GetUninitializedObject(type);
#endif
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
        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
}

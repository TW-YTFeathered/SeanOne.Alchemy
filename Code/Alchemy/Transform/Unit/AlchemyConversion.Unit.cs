// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SeanOne.Alchemy;

partial class Alchemy
{
    // 抽象層，提供單一數值或清單的轉換方法
    private static object ConvertUnit(object input, string instruction, IReadOnlyDictionary<string, Func<double, double>> actions)
    {
        // 輸入值不能為 null
        if (input is null)
            throw new ArgumentNullException(nameof(input));

        // 若指令為空或空白，直接回傳原值，不進行轉換
        if (string.IsNullOrWhiteSpace(instruction))
            return input;

        // 處理集合型態 (含陣列與一般泛型集合)
        if (input is IEnumerable list && input is not string)
            return ConvertList(list, instruction, actions);
        else
        {
            // 單一數值處理
            double value = Convert.ToDouble(input);
            double result = ConvertSingleValue(value, instruction, actions);
            return Convert.ChangeType(result, input.GetType());
        }
    }

    // 私有輔助方法: 轉換單一數值
    private static double ConvertSingleValue(double value, string instruction, IReadOnlyDictionary<string, Func<double, double>> actions)
    {
        if (!actions.TryGetValue(instruction, out var action))
            throw new ArgumentException($"Unknown conversion instruction: {instruction}", nameof(instruction));

        return action(value);
    }

    // 私有輔助方法: 轉換 IEnumerable 集合
    private static object ConvertList(IEnumerable list, string instruction, IReadOnlyDictionary<string, Func<double, double>> actions)
    {
        // 從靜態字典中取得對應的轉換函式，若找不到對應的轉換函式，則拋出異常
        if (!actions.TryGetValue(instruction, out var action))
            throw new ArgumentException($"Unknown conversion instruction: {instruction}", nameof(instruction));

        Type listType = list.GetType();
        bool isArray = listType.IsArray;

        // 取得元素型別
        Type elementType;
        if (isArray)
        {
            elementType = listType.GetElementType();
        }
        else
        {
            var genericInterface = listType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            elementType = genericInterface?.GetGenericArguments().FirstOrDefault() ?? typeof(object);
        }

        // 建立目標集合容器
        IList resultList;
        if (isArray)
        {
            int count = (list as Array).Length;
            resultList = Array.CreateInstance(elementType, count);
        }
        else
        {
            Type targetListType = typeof(List<>).MakeGenericType(elementType);
            resultList = Activator.CreateInstance(targetListType) as IList;
        }

        int index = 0;
        foreach (var item in list)
        {
            // 元素為 null 時無法轉換
            if (item is null)
                throw new InvalidOperationException($"Element at index {index} is null, cannot convert");

            // 若集合原本就是特定型別 (如 int)，強制依 elementType 轉換；若為 object 則動態抓取 item.GetType()
            Type targetType = elementType == typeof(object) ? item.GetType() : elementType;

            try
            {
                double value = Convert.ToDouble(item);
                double result = action(value);

                object convertedValue = Convert.ChangeType(result, targetType);

                if (isArray)
                    resultList[index] = convertedValue;
                else
                    resultList.Add(convertedValue);
            }
            catch (FormatException ex)
            {
                // 當元素無法轉為 double 時拋出
                throw new InvalidOperationException($"Element at index {index} ('{item}') cannot be converted to a numeric value", ex);
            }
            catch (InvalidCastException ex)
            {
                // 當型別轉換失敗時拋出
                throw new InvalidOperationException($"Element at index {index} (type: {targetType.Name}) cannot be converted to a numeric value", ex);
            }
            catch (OverflowException ex)
            {
                // 當運算結果超出目標型別的範圍時拋出
                throw new InvalidOperationException($"Conversion result exceeds the range of {targetType.Name}", ex);
            }

            index++;
        }

        return resultList;
    }
}

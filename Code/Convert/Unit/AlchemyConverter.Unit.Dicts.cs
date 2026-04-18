// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections.Generic;
using static SeanOne.Alchemy.UnitConversions.UnitConversion;

namespace SeanOne.Alchemy
{
    internal static class AlchemyConverterUnitDict
    {
        static AlchemyConverterUnitDict()
        {
            var comparer = StringComparer.InvariantCultureIgnoreCase;

            InitTemperatureDict(comparer);
            InitWeightDict(comparer);
        }

        private static void InitTemperatureDict(StringComparer comparer)
        {
            var dict = new Dictionary<string, Func<double, double>>(comparer);
            var units = Enum.GetValues(typeof(TemperatureUnit));

            foreach (TemperatureUnit from in units)
            {
                foreach (TemperatureUnit to in units)
                {
                    if (from == to) continue;  // 不需要自己轉自己

                    // 建立轉換委派 (捕獲 from 和 to)
                    Func<double, double> convert = value => TemperatureConverter.Convert(value, from, to);

                    // 兩種格式：XtoY 和 X->Y
                    string keyTo = $"{from}To{to}";
                    string keyArrow = $"{from}->{to}";

                    dict[keyTo] = convert;
                    dict[keyArrow] = convert;
                }
            }

            ActionsTemperature = dict;
        }

        private static void InitWeightDict(StringComparer comparer)
        {
            var dict = new Dictionary<string, Func<double, double>>(comparer);
            var units = Enum.GetValues(typeof(WeightUnit));

            foreach (WeightUnit from in units)
            {
                foreach (WeightUnit to in units)
                {
                    if (from == to) continue;  // 不需要自己轉自己

                    // 建立轉換委派 (捕獲 from 和 to)
                    Func<double, double> convert = value => WeightConverter.Convert(value, from, to);

                    // 兩種格式：XtoY 和 X->Y
                    string keyTo = $"{from}To{to}";
                    string keyArrow = $"{from}->{to}";

                    dict[keyTo] = convert;
                    dict[keyArrow] = convert;
                }
            }

            ActionsWeight = dict;
        }

        /// <summary>
        /// 溫度轉換指令與對應函數的映射字典
        /// 支援兩種指令格式: 
        /// <list type="bullet">
        /// <item><description>"XtoY" 格式: 例如 "KtoC" (凱氏轉攝氏)、"FtoC" (華氏轉攝氏)</description></item>
        /// <item><description>"X->Y" 格式: 例如 "K->C"、 "F->C"，使用箭頭符號增強可讀性</description></item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// 字典使用 <see cref="StringComparer.InvariantCultureIgnoreCase"/> 進行比較，
        /// 因此指令不區分大小寫 (例如 "ktoC"、"K->c" 皆可正確匹配)
        /// </remarks>
        public static IReadOnlyDictionary<string, Func<double, double>> ActionsTemperature { get; private set; }

        public static IReadOnlyDictionary<string, Func<double, double>> ActionsWeight { get; private set; }
    }
}

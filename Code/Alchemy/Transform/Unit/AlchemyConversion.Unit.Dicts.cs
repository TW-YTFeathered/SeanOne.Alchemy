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

            ActionsTemperature = InitUnitDict<TemperatureUnit>(comparer, TemperatureConverter.Convert);
            ActionsWeight = InitUnitDict<WeightUnit>(comparer, WeightConverter.Convert);
            ActionsLength = InitUnitDict<LengthUnit>(comparer, LengthConverter.Convert);
        }

        private static Dictionary<string, Func<double, double>> InitUnitDict<T>(StringComparer comparer, Func<double, T, T, double> convertFunc) where T : struct, Enum
        {
            var dict = new Dictionary<string, Func<double, double>>(comparer);
            var units = Enum.GetValues(typeof(T));

            foreach (T from in units)
            {
                foreach (T to in units)
                {
                    if (EqualityComparer<T>.Default.Equals(from, to)) continue; // 不需要自己轉自己

                    Func<double, double> convert = value => convertFunc(value, from, to);

                    dict[$"{from}To{to}"] = convert;
                    dict[$"{from}->{to}"] = convert;
                }
            }

            return dict;
        }

        /// <summary>
        /// 處理溫度轉換的字典
        /// </summary>
        public static IReadOnlyDictionary<string, Func<double, double>> ActionsTemperature { get; private set; }

        /// <summary>
        /// 處理重量轉換的字典
        /// </summary>
        public static IReadOnlyDictionary<string, Func<double, double>> ActionsWeight { get; private set; }

        /// <summary>
        /// 處理長度轉換的字典
        /// </summary>
        public static IReadOnlyDictionary<string, Func<double, double>> ActionsLength { get; private set; }
    }
}

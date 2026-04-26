// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections.Generic;

namespace SeanOne.Alchemy.UnitConversions
{
    partial class UnitConversion
    {
        public enum TemperatureUnit
        {
            C, // 攝氏 (C)
            F, // 華氏 (F)
            K, // 凱氏 (K)
        }

        /// <summary>
        /// 轉換成不同常見溫標 (結構體)
        /// 注意: 不用第三方庫是因為這樣能更好的控制變量
        /// </summary>
        public readonly struct TemperatureConverter
        {
            // 預定義常數，保持邏輯清晰
            private const double KelvinOffset = 273.15;
            private const double FahrenheitOffset = 32.0;
            private const double FahrenheitFactor = 1.8; // 即 9/5

            /// <summary>
            /// 定義相對於凱氏溫度 (K) 的轉換係數
            /// 公式: Kelvin = (Value + AddFirst) * Multiplier + AddSecond
            /// </summary>
            private readonly struct ConversionRule
            {
                public double Ratio { get; }   // 比例
                public double Offset { get; }  // 與凱氏溫標的位移差

                public ConversionRule(double ratio, double offset)
                {
                    Ratio = ratio;
                    Offset = offset;
                }
            }

            // 使用 Dict 儲存規則: 所有單位都先定義「如何轉為凱氏溫度」
            private static readonly IReadOnlyDictionary<TemperatureUnit, ConversionRule> Rules =
                new Dictionary<TemperatureUnit, ConversionRule>
                {
                    // 攝氏: K = C + 273.15 (比例為 1)
                    [TemperatureUnit.C] = new ConversionRule(1.0, KelvinOffset),

                    // 華氏: K = (F - 32) / 1.8 + 273.15
                    // 拆解成: F * (1/1.8) + (273.15 - 32/1.8)
                    [TemperatureUnit.F] = new ConversionRule(1.0 / FahrenheitFactor, KelvinOffset - (FahrenheitOffset / FahrenheitFactor)),

                    // 凱氏: K = K * 1 + 0
                    [TemperatureUnit.K] = new ConversionRule(1.0, 0.0)
                };

            /// <summary> 檢查是否為有效凱氏溫度 (非 NaN、非無限大，且不低於絕對零度)</summary>
            /// <param name="k">凱氏溫度值 (K)</param>
            /// <exception cref="ArgumentException">溫度值無效 (NaN 或 Infinity)或低於絕對零度時拋出</exception>
            private static void ThrowIfInvalidKelvin(double k)
            {
                if (double.IsNaN(k) || double.IsInfinity(k))
                    throw new ArgumentException("Temperature value is not a valid number (NaN or Infinity).");
                if (k < 0)
                    throw new ArgumentException("Temperature below absolute zero is not physically possible.");
            }

            /// <summary>
            /// 通用溫度轉換 (含絕對零度檢查)
            /// </summary>
            /// <param name="value">原始數值</param>
            /// <param name="from">原始單位</param>
            /// <param name="to">目標單位</param>
            /// <returns>轉換後的數值</returns>
            /// <exception cref="ArgumentException">轉換後的中間凱氏溫度低於絕對零度或無效時拋出</exception>
            public static double Convert(double value, TemperatureUnit from, TemperatureUnit to)
            {
                double kelvin = Convert_Core(value, from, TemperatureUnit.K);
                ThrowIfInvalidKelvin(kelvin);

                if (from == to) return value;
                
                return Convert_Core(kelvin, TemperatureUnit.K, to);
            }

            /// <summary>
            /// 通用溫度轉換 (純數學公式，無任何檢查)
            /// </summary>
            /// <param name="value">原始數值</param>
            /// <param name="from">原始單位</param>
            /// <param name="to">目標單位</param>
            /// <returns>轉換後的數值</returns>
            public static double Convert_Core(double value, TemperatureUnit from, TemperatureUnit to)
            {
                // 先轉為凱氏溫度
                double kelvin = Rules[from].Ratio * value + Rules[from].Offset;
                // 再從凱氏轉為目標單位
                return (kelvin - Rules[to].Offset) / Rules[to].Ratio;
            }
        }
    }
}

// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections.Generic;

namespace SeanOne.Alchemy.UnitConversions
{
    partial class UnitConversion
    {
        /// <summary>
        /// 重量單位列舉
        /// </summary>
        public enum WeightUnit
        {
            Milligram,   // 毫克 (mg)
            Centigram,   // 厘克 (cg)
            Decigram,    // 分克 (dg)
            Gram,        // 公克 (g)
            Decagram,    // 十克 (dag)
            Hectogram,   // 百克 (hg)
            Kilogram,    // 公斤 (kg)
            Tonne,       // 公噸 (t)
            Ounce,       // 盎司 (oz)
            Pound,       // 英磅 (lb)
            Stone,       // 英石 (st)
            ShortTon,    // 短噸 (shortTon)
            LongTon      // 長噸 (longTon)
        }

        /// <summary>
        /// 重量單位轉換器 (結構體)
        /// </summary>
        public readonly struct WeightConverter
        {
            // 公制基本常數 (以公克為基準)
            private const double MilligramsPerGram = 1000.0;   // 1 公克 = 1000 毫克
            private const double CentigramsPerGram = 100.0;    // 1 公克 = 100 公毫 (厘克)
            private const double DecigramsPerGram = 10.0;      // 1 公克 = 10 公銖 (分克)
            private const double GramsPerDecagram = 10.0;      // 1 公錢 = 10 公克
            private const double GramsPerHectogram = 100.0;    // 1 百克 = 100 公克
            private const double GramsPerKilogram = 1000.0;    // 1 公斤 = 1000 公克
            private const double KilogramsPerTonne = 1000.0;   // 1 公噸 = 1000 公斤

            // 英制 / 美制
            private const double GramsPerOunce = 28.349523125;       // 1 盎司 = 28.349523125 公克
            private const double GramsPerPound = 453.59237;          // 1 英磅 = 453.59237 公克
            private const double KilogramsPerStone = 6.35029318;     // 1 英石 = 6.35029318 公斤
            private const double KilogramsPerShortTon = 907.18474;   // 1 短噸 = 907.18474 公斤
            private const double KilogramsPerLongTon = 1016.0469088; // 1 長噸 = 1016.0469088 公斤

            // 各單位對應的「1 單位 = ? 公克」的係數
            private static readonly IReadOnlyDictionary<WeightUnit, double> ToGramFactor =
                new Dictionary<WeightUnit, double>
                {
                    [WeightUnit.Milligram] = 1.0 / MilligramsPerGram,                // 0.001
                    [WeightUnit.Centigram] = 1.0 / CentigramsPerGram,                // 0.01
                    [WeightUnit.Decigram] = 1.0 / DecigramsPerGram,                  // 0.1
                    [WeightUnit.Gram] = 1.0,                                         // 1
                    [WeightUnit.Decagram] = GramsPerDecagram,                        // 10
                    [WeightUnit.Hectogram] = GramsPerHectogram,                      // 100
                    [WeightUnit.Kilogram] = GramsPerKilogram,                        // 1000
                    [WeightUnit.Tonne] = KilogramsPerTonne * GramsPerKilogram,       // 1,000,000
                    [WeightUnit.Ounce] = GramsPerOunce,                              // 28.349523125
                    [WeightUnit.Pound] = GramsPerPound,                              // 453.59237
                    [WeightUnit.Stone] = KilogramsPerStone * GramsPerKilogram,       // 6350.29318
                    [WeightUnit.ShortTon] = KilogramsPerShortTon * GramsPerKilogram, // 907184.74
                    [WeightUnit.LongTon] = KilogramsPerLongTon * GramsPerKilogram    // 1016046.9088
                };

            /// <summary> 檢查是否為有效重量值 (非 NaN、非無限大，且不小於零)</summary>
            /// <param name="value">輸入數值</param>
            /// <exception cref="ArgumentException">重量值無效 (NaN 或 Infinity)或為負數時拋出</exception>
            private static void ThrowIfNegative(double value)
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                    throw new ArgumentException("Weight value is not a valid number (NaN or Infinity).");
                if (value < 0)
                    throw new ArgumentException("Weight cannot be negative.");
            }

            /// <summary>
            /// 通用重量轉換 (含負數檢查)
            /// </summary>
            /// <param name="value">原始數值</param>
            /// <param name="from">原始單位</param>
            /// <param name="to">目標單位</param>
            /// <returns>轉換後的數值</returns>
            /// <exception cref="ArgumentException">數值小於零時拋出</exception>
            public static double Convert(double value, WeightUnit from, WeightUnit to)
            {
                ThrowIfNegative(value);

                if (from == to) return value;

                return Convert_Core(value, from, to);
            }

            /// <summary>
            /// 通用重量轉換 (純數學公式，無負數檢查)
            /// </summary>
            /// <param name="value">原始數值</param>
            /// <param name="from">原始單位</param>
            /// <param name="to">目標單位</param>
            /// <returns>轉換後的數值</returns>
            public static double Convert_Core(double value, WeightUnit from, WeightUnit to)
            {
                double gram = value * ToGramFactor[from];
                return gram / ToGramFactor[to];
            }
        }
    }
}

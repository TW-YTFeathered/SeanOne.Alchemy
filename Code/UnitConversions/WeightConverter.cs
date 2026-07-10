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
            // 公制單位 (Metric System)
            Mg,          // 毫克 (Milligram)
            Cg,          // 厘克 (Centigram)
            Dg,          // 分克 (Decigram)
            G,           // 公克 (Gram)
            Dag,         // 十克 (Decagram)
            Hg,          // 百克 (Hectogram)
            Kg,          // 公斤 (Kilogram)
            T,           // 公噸 (Tonne)

            // 英美制質量單位 (Imperial / US Customary)
            Oz,          // 盎司 (Ounce)
            Lb,          // 英磅 (Pound)
            St,          // 英石 (Stone)
            ShortTon,    // 短噸 (Short Ton / US Ton)
            LongTon      // 長噸 (Long Ton / Imperial Ton)
        }

        /// <summary>
        /// 重量單位轉換器 (結構體)
        /// 注意: 不用第三方庫是因為這樣能更好的控制變量
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
                    [WeightUnit.Mg] = 1.0 / MilligramsPerGram,                       // 0.001
                    [WeightUnit.Cg] = 1.0 / CentigramsPerGram,                       // 0.01
                    [WeightUnit.Dg] = 1.0 / DecigramsPerGram,                        // 0.1
                    [WeightUnit.G] = 1.0,                                            // 1
                    [WeightUnit.Dag] = GramsPerDecagram,                             // 10
                    [WeightUnit.Hg] = GramsPerHectogram,                             // 100
                    [WeightUnit.Kg] = GramsPerKilogram,                              // 1000
                    [WeightUnit.T] = KilogramsPerTonne * GramsPerKilogram,           // 1,000,000
                    [WeightUnit.Oz] = GramsPerOunce,                                 // 28.349523125
                    [WeightUnit.Lb] = GramsPerPound,                                 // 453.59237
                    [WeightUnit.St] = KilogramsPerStone * GramsPerKilogram,          // 6350.29318
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
                // 先轉為公克
                double gram = value * ToGramFactor[from];
                // 再從公克轉為目標單位
                return gram / ToGramFactor[to];
            }
        }
    }
}

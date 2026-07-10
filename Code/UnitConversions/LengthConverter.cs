// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections.Generic;

namespace SeanOne.Alchemy.UnitConversions
{
    partial class UnitConversion
    {
        /// <summary>
        /// 長度單位列舉
        /// </summary>
        public enum LengthUnit
        {
            // 公制單位
            A,    // 埃 (Angstrom)
            NM,   // 奈米 (Nanometer)
            UM,   // 微米 (Micrometer)
            MM,   // 公釐 (Millimeter)
            CM,   // 公分 (Centimeter)
            M,    // 公尺 (Meter)
            KM,   // 公里 (Kilometer)

            // 英制與其他單位
            IN,   // 英吋 (Inch)
            FT,   // 英呎 (Foot)
            YD,   // 碼 (Yard)
            MI,   // 英哩 (Mile)
            NMI   // 海哩 (Nautical Mile)
        }

        /// <summary>
        /// 長度單位轉換器 (結構體)
        /// 注意: 不用第三方庫是因為這樣能更好的控制變量
        /// </summary>
        public readonly struct LengthConverter
        {
            // 公制基本常數 (以公分為基準)
            private const double AngstromsPerCentimeter = 1e8;          // 1 公分 = 10^8 埃
            private const double NanometersPerCentimeter = 1e7;         // 1 公分 = 10^7 奈米
            private const double MicrometersPerCentimeter = 1e4;        // 1 公分 = 10^4 微米
            private const double MillimetersPerCentimeter = 10.0;       // 1 公分 = 10 公釐
            private const double CentimetersPerMeter = 100.0;           // 1 公尺 = 100 公分
            private const double MetersPerKilometer = 1000.0;           // 1 公里 = 1000 公尺

            // 英制 / 其他常數
            private const double CentimetersPerInch = 2.54;             // 1 英吋 = 2.54 公分 (精確定義)
            private const double InchesPerFoot = 12.0;                  // 1 英呎 = 12 英吋
            private const double FeetPerYard = 3.0;                     // 1 碼 = 3 英呎
            private const double FeetPerMile = 5280.0;                  // 1 英哩 = 5280 英呎
            private const double MetersPerNauticalMile = 1852.0;        // 1 海哩 = 1852 公尺

            // 各單位對應的「1 單位 = ? 公分」的係數
            private static readonly IReadOnlyDictionary<LengthUnit, double> ToCentimeterFactor =
                new Dictionary<LengthUnit, double>
                {
                    // 公制單位
                    [LengthUnit.A] = 1.0 / AngstromsPerCentimeter,                      // 1 埃 = 10^-8 公分
                    [LengthUnit.NM] = 1.0 / NanometersPerCentimeter,                    // 1 奈米 = 10^-7 公分
                    [LengthUnit.UM] = 1.0 / MicrometersPerCentimeter,                   // 1 微米 = 10^-4 公分
                    [LengthUnit.MM] = 1.0 / MillimetersPerCentimeter,                   // 1 公釐 = 0.1 公分
                    [LengthUnit.CM] = 1.0,                                              // 1 公分 = 1 公分 (基準單位)
                    [LengthUnit.M] = CentimetersPerMeter,                               // 1 公尺 = 100 公分
                    [LengthUnit.KM] = CentimetersPerMeter * MetersPerKilometer,         // 1 公里 = 100,000 公分

                    // 英制與其他
                    [LengthUnit.IN] = CentimetersPerInch,                               // 1 英吋 = 2.54 公分
                    [LengthUnit.FT] = CentimetersPerInch * InchesPerFoot,               // 1 英呎 = 30.48 公分
                    [LengthUnit.YD] = CentimetersPerInch * InchesPerFoot * FeetPerYard, // 1 碼 = 91.44 公分
                    [LengthUnit.MI] = CentimetersPerInch * InchesPerFoot * FeetPerMile, // 1 英哩 = 160934.4 公分
                    [LengthUnit.NMI] = CentimetersPerMeter * MetersPerNauticalMile      // 1 海哩 = 185200 公分
                };

            /// <summary> 檢查是否為有效長度值 (非 NaN、非無限大，且不小於零)</summary>
            /// <param name="value">輸入數值</param>
            /// <exception cref="ArgumentException">長度值無效 (NaN 或 Infinity)或為負數時拋出</exception>
            private static void ThrowIfNegative(double value)
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                    throw new ArgumentException("Length value is not a valid number (NaN or Infinity).");
                if (value < 0)
                    throw new ArgumentException("Length cannot be negative.");
            }

            public static double Convert(double value, LengthUnit from, LengthUnit to)
            {
                ThrowIfNegative(value);

                if (from == to) return value;

                return Convert_Core(value, from, to);
            }

            public static double Convert_Core(double value, LengthUnit from, LengthUnit to)
            {
                // 先轉為公分
                double centimeters = value * ToCentimeterFactor[from];
                // 再從公分轉為目標單位
                return centimeters / ToCentimeterFactor[to];
            }
        }
    }
}

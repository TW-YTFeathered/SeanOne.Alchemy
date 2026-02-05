using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SeanOne.Alchemy.Utility
{
    /// <summary>
    /// 將指定輸入轉換成 Unicode 值
    /// 注意: 部分情況直接使用 C# 數值轉換更快更準，可能看似非標準 Unicode
    /// </summary>
    internal static class ConvertToUnicode
    {
        /// <summary>
        /// 將字串中的所有 \uXXXX 轉義序列轉換為其對應的 Unicode 字符
        /// </summary>
        /// <param name="input"> 要被轉換的字串 </param>
        public static string DecodeUnicodeEscapes(string input)
        {
            // 使用正則表達式來尋找所有的 \\uXXXX 序列
            return Regex.Replace(input, @"\\u([0-9a-fA-F]{4})", match =>
            {
                try
                {
                    // 將十六進位值轉換為對應的 Unicode 字符
                    return ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString();
                }
                catch
                {
                    // 如果轉換失敗，保留原始字串
                    return match.Value;
                }
            });
        }

        /// <summary>
        /// 將字串中的所有 \X 轉義序列轉換為其對應的 Unicode 字符
        /// </summary>
        /// <param name="input"> 要被轉換的字串 </param>
        public static string Unescape(string input)
        {
            // 如果為空，直接返回 string.Empty
            if (string.IsNullOrEmpty(input)) return string.Empty; 

            var sb = new StringBuilder(); // 用於存放結果的字串

            int count = input.Count(); // 取得字串長度

            for (int i = 0; i < count; i++)
            {
                char thisChar = input[i]; // 取得當前字符

                if (thisChar == '\\' && i + 1 < count)
                {
                    char nextChar = input[i + 1]; // 取得下一個字符

                    switch (nextChar) 
                    {
                        case '0': sb.Append('\0'); break;
                        case 'a': sb.Append('\a'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append(Environment.NewLine); break; // 避免環境差異，使用 Environment.NewLine
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'v': sb.Append('\v'); break;
                        case '\\': sb.Append('\\'); break;
                        case '\'': sb.Append('\''); break;
                        case '\"': sb.Append('\"'); break;
                        default: sb.Append(thisChar).Append(nextChar); break;
                    }

                    i++;
                }
                else
                {
                    sb.Append(thisChar); // 非轉義字符，直接添加
                }
            }

            return sb.ToString(); // 回傳轉換結果
        }
    }

    /// <summary>
    /// 轉換成不同常見溫標
    /// 注意: 不用第三方庫是因為這樣能更好的控制變量
    /// </summary>
    internal class TemperatureConverter
    {
        //private const double AbsoluteZeroC = -273.15;
        // 攝氏零度相對於絕對溫度的偏移量
        private const double KelvinOffset = 273.15;
        // 攝氏零度對應的華氏溫度之偏移量
        private const double FahrenheitOffset = 32.0;
        // 攝氏與華氏的比例 (9/5)
        private const double FahrenheitFactor = 1.8;

        private void ThrowIfBelowAbsoluteZero(double k)
        {
            if (k < 0)
                throw new ArgumentException("Temperature below absolute zero is not physically possible.");
        }

        #region Abstract version (checking absolute zero)
        public double FtoK(double f)
        {
            double k = FtoK_Core(f);
            ThrowIfBelowAbsoluteZero(k);
            return k;
        }

        public double FtoC(double f)
        {
            double k = FtoK_Core(f);
            ThrowIfBelowAbsoluteZero(k);
            return FtoC_Core(f);
        }

        public double KtoF(double k)
        {
            ThrowIfBelowAbsoluteZero(k);
            return KtoF_Core(k);
        }

        public double KtoC(double k)
        {
            ThrowIfBelowAbsoluteZero(k);
            return KtoC_Core(k);
        }

        public double CtoK(double c)
        {
            double k = CtoK_Core(c);
            ThrowIfBelowAbsoluteZero(k);
            return k;
        }

        public double CtoF(double c)
        {
            double k = CtoK_Core(c);
            ThrowIfBelowAbsoluteZero(k);
            return CtoF_Core(c);
        }
        #endregion

        #region Core version (pure mathematical conversion)
        public double FtoK_Core(double f) => (f - FahrenheitOffset) / FahrenheitFactor + KelvinOffset;
        public double FtoC_Core(double f) => (f - FahrenheitOffset) / FahrenheitFactor;
        public double KtoF_Core(double k) => (k - KelvinOffset) * FahrenheitFactor + FahrenheitOffset;
        public double KtoC_Core(double k) => k - KelvinOffset;
        public double CtoK_Core(double c) => c + KelvinOffset;
        public double CtoF_Core(double c) => c * FahrenheitFactor + FahrenheitOffset;
        #endregion
    }
}

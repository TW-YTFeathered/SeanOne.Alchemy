// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;
using System.Text;

#if NET6_0_OR_GREATER
using System.Buffers;
#endif

namespace SeanOne.Alchemy
{
    partial class Alchemy
    {
        /// <summary>
        /// 處理字典集合 (優化過後)
        /// </summary>
        /// <param name="dictionary"> 目標字典 </param>
        /// <param name="dictFormat"> 字典格式 </param>
        /// <param name="keyFormat"> 字典的鍵格式 </param>
        /// <param name="valueFormat"> 字典的值格式 </param>
        /// <param name="begin"> 每次跌代前加的字串</param>
        /// <param name="end"> 每次跌代後加的字串(如果是倒數第二個且 final_pair_separator 為空字串，或是最後一個且 exclude_last_end 為 true，則不加) </param>
        /// <param name="final_pair_separator"> 用於倒數第二個與最後一個項目之間的連接字串 </param>
        /// <param name="prefix"> 最前面的前綴，會加在整個字典處理結果的開頭(即使字典為空也會添加) </param>
        /// <param name="suffix"> 最後面的後綴，會加在整個字典處理結果的結尾(即使字典為空也會添加) </param>
        /// <param name="exclude_last_end"> 是否排除最後一個項目的 end 字串 </param>
        private static string FE_ProcessDictionary_Optimized(IDictionary dictionary,
            string dictFormat, string keyFormat, string valueFormat,
            string begin, string end, string final_pair_separator,
            string prefix, string suffix,
            bool exclude_last_end)
        {
#if NET6_0_OR_GREATER
            return FE_ProcessDictionary_Optimized_Net6(dictionary, 
                dictFormat, keyFormat, valueFormat,
                begin, end, final_pair_separator,
                prefix, suffix,
                exclude_last_end);
#else
            return FE_ProcessDictionary_Optimized_Legacy(dictionary,
                dictFormat, keyFormat, valueFormat,
                begin, end, final_pair_separator,
                prefix, suffix,
                exclude_last_end);
#endif
        }

#if NET6_0_OR_GREATER
        private static string FE_ProcessDictionary_Optimized_Net6(IDictionary dictionary,
            string dictFormat, string keyFormat, string valueFormat,
            string begin, string end, string final_pair_separator,
            string prefix, string suffix,
            bool exclude_last_end)
        {
            if (dictionary == null) return prefix + suffix;
            if (string.IsNullOrEmpty(dictFormat))
                throw new ArgumentNullException("'dict-format' parameter is required when processing dictionaries.");

            bool hasFps = !string.IsNullOrEmpty(final_pair_separator);

            ReadOnlySpan<char> beginSpan = begin.AsSpan();
            ReadOnlySpan<char> endSpan = end.AsSpan();
            ReadOnlySpan<char> fpsSpan = hasFps ? final_pair_separator.AsSpan() : default;

            char[] buf = ArrayPool<char>.Shared.Rent(512);
            int len = 0;

            var enumerator = dictionary.GetEnumerator();

            try
            {
                if (!enumerator.MoveNext()) return prefix + suffix;

                DictionaryEntry currentItem = (DictionaryEntry)enumerator.Current;
                bool hasNext = enumerator.MoveNext();

                AppendSpan(ref buf, ref len, prefix.AsSpan());

                if (!hasNext)
                {
                    AppendSpan(ref buf, ref len, beginSpan);
                    AppendSpan(ref buf, ref len, Dict_Format(
                        dictFormat,
                        FormatObject(currentItem.Key, keyFormat),
                        FormatObject(currentItem.Value, valueFormat)
                        ).AsSpan());
                    if (!exclude_last_end) AppendSpan(ref buf, ref len, endSpan);
                    AppendSpan(ref buf, ref len, suffix.AsSpan());
                    return new string(buf, 0, len);
                }

                AppendSpan(ref buf, ref len, beginSpan);
                AppendSpan(ref buf, ref len, Dict_Format(
                    dictFormat,
                    FormatObject(currentItem.Key, keyFormat),
                    FormatObject(currentItem.Value, valueFormat)
                    ).AsSpan());

                while (true)
                {
                    DictionaryEntry nextItem = (DictionaryEntry)enumerator.Current;
                    hasNext = enumerator.MoveNext();

                    if (!hasNext)
                    {
                        AppendSpan(ref buf, ref len, hasFps ? fpsSpan : endSpan);
                        AppendSpan(ref buf, ref len, beginSpan);
                        AppendSpan(ref buf, ref len, Dict_Format(
                            dictFormat,
                            FormatObject(nextItem.Key, keyFormat),
                            FormatObject(nextItem.Value, valueFormat)
                            ).AsSpan());
                        if (!exclude_last_end) AppendSpan(ref buf, ref len, endSpan);
                        break;
                    }

                    AppendSpan(ref buf, ref len, endSpan);
                    AppendSpan(ref buf, ref len, beginSpan);
                    AppendSpan(ref buf, ref len, Dict_Format(
                        dictFormat,
                        FormatObject(nextItem.Key, keyFormat),
                        FormatObject(nextItem.Value, valueFormat)
                        ).AsSpan());

                    // 因為目前迴圈內沒有用，所以先註解掉
                    //currentItem = nextItem;
                }

                AppendSpan(ref buf, ref len, suffix.AsSpan());
                return new string(buf, 0, len);
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
                ArrayPool<char>.Shared.Return(buf);
            }
        }
#else
        private static string FE_ProcessDictionary_Optimized_Legacy(IDictionary dictionary,
            string dictFormat, string keyFormat, string valueFormat,
            string begin, string end, string final_pair_separator,
            string prefix, string suffix,
            bool exclude_last_end)
        {
            if (dictionary == null) return prefix + suffix;
            if (string.IsNullOrEmpty(dictFormat))
                throw new ArgumentNullException("'dict-format' parameter is required when processing dictionaries.");

            var results = new StringBuilder();
            var enumerator = dictionary.GetEnumerator();

            try
            {
                if (!enumerator.MoveNext()) return prefix + suffix;

                DictionaryEntry currentItem = (DictionaryEntry)enumerator.Current;
                bool hasNextItem = enumerator.MoveNext();

                // 單元素
                if (!hasNextItem)
                {
                    results.Append(prefix).Append(begin);

                    string keyStr = FormatObject(currentItem.Key, keyFormat);
                    string valueStr = FormatObject(currentItem.Value, valueFormat);
                    results.Append(Dict_Format(dictFormat, keyStr, valueStr));

                    if (!exclude_last_end) results.Append(end);

                    results.Append(suffix);
                    return results.ToString();
                }

                // 多元素
                results.Append(prefix);

                while (hasNextItem)
                {
                    DictionaryEntry nextItem = (DictionaryEntry)enumerator.Current;
                    hasNextItem = enumerator.MoveNext();

                    string keyStr = FormatObject(currentItem.Key, keyFormat);
                    string valueStr = FormatObject(currentItem.Value, valueFormat);
                    string formatted = Dict_Format(dictFormat, keyStr, valueStr);

                    if (!hasNextItem)
                    {
                        // 倒數第二個
                        if (!string.IsNullOrEmpty(final_pair_separator))
                            results.Append(begin).Append(formatted).Append(final_pair_separator);
                        else
                            results.Append(begin).Append(formatted).Append(end);

                        // 最後一個
                        string lastKeyStr = FormatObject(nextItem.Key, keyFormat);
                        string lastValueStr = FormatObject(nextItem.Value, valueFormat);
                        results.Append(begin).Append(Dict_Format(dictFormat, lastKeyStr, lastValueStr));

                        if (!exclude_last_end) results.Append(end);
                    }
                    else
                    {
                        // 中間元素
                        results.Append(begin).Append(formatted).Append(end);
                    }

                    currentItem = nextItem;
                }
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }

            results.Append(suffix);
            return results.ToString();
        }
#endif

        /// <summary>
        /// 處理普通集合 (優化過後)
        /// </summary>
        /// <param name="enumerable"> 目標集合 </param>
        /// <param name="format"> 指定集合的格式化方式 </param>
        /// <param name="begin"> 每次跌代前加的字串</param>
        /// <param name="end"> 每次跌代後加的字串(如果是倒數第二個且 final_pair_separator 為空字串，或是最後一個且 exclude_last_end 為 true，則不加) </param>
        /// <param name="final_pair_separator"> 用於倒數第二個與最後一個項目之間的連接字串 </param>
        /// <param name="prefix"> 最前面的前綴，會加在整個字典處理結果的開頭(即使字典為空也會添加) </param>
        /// <param name="suffix"> 最後面的後綴，會加在整個字典處理結果的結尾(即使字典為空也會添加) </param>
        /// <param name="exclude_last_end"> 是否排除最後一個項目的 end 字串 </param>
        private static string FE_ProcessEnumerable_Optimized(IEnumerable enumerable,
            string format,
            string begin, string end, string final_pair_separator,
            string prefix, string suffix,
            bool exclude_last_end)
        {
#if NET6_0_OR_GREATER
            return FE_ProcessEnumerable_Optimized_Net6(enumerable, format, begin, end, final_pair_separator, prefix, suffix, exclude_last_end);
#else
            return FE_ProcessEnumerable_Optimized_Legacy(enumerable, format, begin, end, final_pair_separator, prefix, suffix, exclude_last_end);
#endif
        }

#if NET6_0_OR_GREATER
        private static string FE_ProcessEnumerable_Optimized_Net6(IEnumerable enumerable,
            string format,
            string begin, string end, string final_pair_separator,
            string prefix, string suffix,
            bool exclude_last_end)
        {
            if (enumerable == null) return prefix + suffix;

            bool hasFormat = !string.IsNullOrEmpty(format);
            bool hasFps = !string.IsNullOrEmpty(final_pair_separator);
            ReadOnlySpan<char> beginSpan = begin.AsSpan();
            ReadOnlySpan<char> endSpan = end.AsSpan();
            ReadOnlySpan<char> fpsSpan = hasFps ? final_pair_separator.AsSpan() : default;

            char[] buf = ArrayPool<char>.Shared.Rent(512);
            int len = 0;
            var enumerator = enumerable.GetEnumerator();
            try
            {
                if (!enumerator.MoveNext())
                    return prefix + suffix;   // buf 和 enumerator 會在 finally 釋放

                object currentItem = enumerator.Current;
                bool hasNext = enumerator.MoveNext();

                AppendSpan(ref buf, ref len, prefix.AsSpan());

                if (!hasNext)
                {
                    AppendSpan(ref buf, ref len, beginSpan);
                    AppendObj(ref buf, ref len, currentItem, format, hasFormat);
                    if (!exclude_last_end) AppendSpan(ref buf, ref len, endSpan);
                    AppendSpan(ref buf, ref len, suffix.AsSpan());
                    return new string(buf, 0, len); // finally 會清理 buf
                }

                AppendSpan(ref buf, ref len, beginSpan);
                AppendObj(ref buf, ref len, currentItem, format, hasFormat);

                while (true)
                {
                    object nextItem = enumerator.Current;
                    hasNext = enumerator.MoveNext();

                    if (!hasNext)
                    {
                        AppendSpan(ref buf, ref len, hasFps ? fpsSpan : endSpan);
                        AppendSpan(ref buf, ref len, beginSpan);
                        AppendObj(ref buf, ref len, nextItem, format, hasFormat);
                        if (!exclude_last_end) AppendSpan(ref buf, ref len, endSpan);
                        break;
                    }

                    AppendSpan(ref buf, ref len, endSpan);
                    AppendSpan(ref buf, ref len, beginSpan);
                    AppendObj(ref buf, ref len, nextItem, format, hasFormat);

                    // 因為目前迴圈內沒有用，所以先註解掉
                    //currentItem = nextItem;
                }

                AppendSpan(ref buf, ref len, suffix.AsSpan());
                return new string(buf, 0, len);
            }
            finally
            {
                // 釋放資源
                (enumerator as IDisposable)?.Dispose();
                ArrayPool<char>.Shared.Return(buf);
            }
        }
#else
        private static string FE_ProcessEnumerable_Optimized_Legacy(IEnumerable enumerable,
            string format,
            string begin, string end, string final_pair_separator,
            string prefix, string suffix,
            bool exclude_last_end)
        {
            if (enumerable == null) return prefix + suffix;

            bool hasFps = !string.IsNullOrEmpty(final_pair_separator);

            var result = new StringBuilder();
            var enumerator = enumerable.GetEnumerator();

            try
            {
                if (!enumerator.MoveNext()) return prefix + suffix;

                object currentItem = enumerator.Current;
                bool hasNext = enumerator.MoveNext();

                result.Append(prefix);

                if (!hasNext)
                {
                    result.Append(begin);
                    result.Append(FormatObject(currentItem, format));
                    if (!exclude_last_end) result.Append(end);
                    result.Append(suffix);
                    return result.ToString();
                }

                // 第一個元素的格式化字串（先不寫入，等迴圈內統一處理）
                string currentStr = FormatObject(currentItem, format);

                while (hasNext)
                {
                    object nextItem = enumerator.Current;
                    hasNext = enumerator.MoveNext();

                    if (!hasNext)
                    {
                        // 倒數第二個元素：使用 final_pair_separator 或 end
                        result.Append(begin).Append(currentStr)
                              .Append(hasFps ? final_pair_separator : end);

                        // 最後一個元素
                        result.Append(begin).Append(FormatObject(nextItem, format));
                        if (!exclude_last_end) result.Append(end);
                    }
                    else
                    {
                        // 一般中間元素
                        result.Append(begin).Append(currentStr).Append(end);
                        currentStr = FormatObject(nextItem, format);
                    }
                }

                return result.Append(suffix).ToString();
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }
#endif
    }
}

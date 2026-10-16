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
        /// <param name="options"> DSL 參數集合 </param>
        private static string FE_ProcessDictionary_Optimized(IDictionary dictionary, in FeOptions options)
        {
#if NET6_0_OR_GREATER
            return FE_ProcessDictionary_Optimized_Net6(dictionary, options);
#else
            return FE_ProcessDictionary_Optimized_Legacy(dictionary, options);
#endif
        }

#if NET6_0_OR_GREATER
        private static string FE_ProcessDictionary_Optimized_Net6(IDictionary dictionary, in FeOptions options)
        {
            if (dictionary is null) return options.prefix + options.suffix;
            if (string.IsNullOrEmpty(options.dict_format))
                throw new ArgumentNullException("'dict-format' parameter is required when processing dictionaries.");

            bool hasFps = !string.IsNullOrEmpty(options.final_pair_separator);

            ReadOnlySpan<char> beginSpan = options.begin.AsSpan();
            ReadOnlySpan<char> endSpan = options.end.AsSpan();
            ReadOnlySpan<char> fpsSpan = hasFps ? options.final_pair_separator.AsSpan() : default;

            char[] buf = ArrayPool<char>.Shared.Rent(512);
            int len = 0;

            var enumerator = dictionary.GetEnumerator();

            try
            {
                if (!enumerator.MoveNext()) return options.prefix + options.suffix;

                DictionaryEntry currentItem = (DictionaryEntry)enumerator.Current;
                bool hasNext = enumerator.MoveNext();

                AppendSpan(ref buf, ref len, options.prefix.AsSpan());

                if (!hasNext)
                {
                    AppendSpan(ref buf, ref len, beginSpan);
                    AppendSpan(ref buf, ref len, Dict_Format(
                        options.dict_format,
                        FormatObject(currentItem.Key, options.key_format),
                        FormatObject(currentItem.Value, options.value_format)
                        ).AsSpan());
                    if (!options.exclude_last_end) AppendSpan(ref buf, ref len, endSpan);
                    AppendSpan(ref buf, ref len, options.suffix.AsSpan());
                    return new(buf, 0, len);
                }

                AppendSpan(ref buf, ref len, beginSpan);
                AppendSpan(ref buf, ref len, Dict_Format(
                    options.dict_format,
                    FormatObject(currentItem.Key, options.key_format),
                    FormatObject(currentItem.Value, options.value_format)
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
                            options.dict_format,
                            FormatObject(nextItem.Key, options.key_format),
                            FormatObject(nextItem.Value, options.value_format)
                            ).AsSpan());
                        if (!options.exclude_last_end) AppendSpan(ref buf, ref len, endSpan);
                        break;
                    }

                    AppendSpan(ref buf, ref len, endSpan);
                    AppendSpan(ref buf, ref len, beginSpan);
                    AppendSpan(ref buf, ref len, Dict_Format(
                        options.dict_format,
                        FormatObject(nextItem.Key, options.key_format),
                        FormatObject(nextItem.Value, options.value_format)
                        ).AsSpan());
                }

                AppendSpan(ref buf, ref len, options.suffix.AsSpan());
                return new(buf, 0, len);
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
                ArrayPool<char>.Shared.Return(buf);
            }
        }
#else
        private static string FE_ProcessDictionary_Optimized_Legacy(IDictionary dictionary, in FeOptions options)
        {
            if (dictionary is null) return options.prefix + options.suffix;
            if (string.IsNullOrEmpty(options.dict_format))
                throw new ArgumentNullException("'dict-format' parameter is required when processing dictionaries.");

            StringBuilder results = new();
            var enumerator = dictionary.GetEnumerator();

            try
            {
                if (!enumerator.MoveNext()) return options.prefix + options.suffix;

                DictionaryEntry currentItem = (DictionaryEntry)enumerator.Current;
                bool hasNextItem = enumerator.MoveNext();

                // 單元素
                if (!hasNextItem)
                {
                    results.Append(options.prefix).Append(options.begin);

                    string keyStr = FormatObject(currentItem.Key, options.key_format);
                    string valueStr = FormatObject(currentItem.Value, options.value_format);
                    results.Append(Dict_Format(options.dict_format, keyStr, valueStr));

                    if (!options.exclude_last_end) results.Append(options.end);

                    results.Append(options.suffix);
                    return results.ToString();
                }

                // 多元素
                results.Append(options.prefix);

                while (hasNextItem)
                {
                    DictionaryEntry nextItem = (DictionaryEntry)enumerator.Current;
                    hasNextItem = enumerator.MoveNext();

                    string keyStr = FormatObject(currentItem.Key, options.key_format);
                    string valueStr = FormatObject(currentItem.Value, options.value_format);
                    string formatted = Dict_Format(options.dict_format, keyStr, valueStr);

                    if (!hasNextItem)
                    {
                        // 倒數第二個
                        if (!string.IsNullOrEmpty(options.final_pair_separator))
                            results.Append(options.begin).Append(formatted).Append(options.final_pair_separator);
                        else
                            results.Append(options.begin).Append(formatted).Append(options.end);

                        // 最後一個
                        string lastKeyStr = FormatObject(nextItem.Key, options.key_format);
                        string lastValueStr = FormatObject(nextItem.Value, options.value_format);
                        results.Append(options.begin).Append(Dict_Format(options.dict_format, lastKeyStr, lastValueStr));

                        if (!options.exclude_last_end) results.Append(options.end);
                    }
                    // 中間元素
                    else
                        results.Append(options.begin).Append(formatted).Append(options.end);

                    currentItem = nextItem;
                }
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }

            results.Append(options.suffix);
            return results.ToString();
        }
#endif

        /// <summary>
        /// 處理普通集合 (優化過後)
        /// </summary>
        /// <param name="enumerable"> 目標集合 </param>
        /// <param name="options"> DSL 參數集合 </param>
        private static string FE_ProcessEnumerable_Optimized(IEnumerable enumerable, in FeOptions options)
        {
#if NET6_0_OR_GREATER
            return FE_ProcessEnumerable_Optimized_Net6(enumerable, options);
#else
            return FE_ProcessEnumerable_Optimized_Legacy(enumerable, options);
#endif
        }

#if NET6_0_OR_GREATER
        private static string FE_ProcessEnumerable_Optimized_Net6(IEnumerable enumerable, in FeOptions options)
        {
            if (enumerable is null) return options.prefix + options.suffix;

            bool hasFormat = !string.IsNullOrEmpty(options.tostring);
            bool hasFps = !string.IsNullOrEmpty(options.final_pair_separator);
            ReadOnlySpan<char> beginSpan = options.begin.AsSpan();
            ReadOnlySpan<char> endSpan = options.end.AsSpan();
            ReadOnlySpan<char> fpsSpan = hasFps ? options.final_pair_separator.AsSpan() : default;

            char[] buf = ArrayPool<char>.Shared.Rent(512);
            int len = 0;
            var enumerator = enumerable.GetEnumerator();
            try
            {
                if (!enumerator.MoveNext())
                    return options.prefix + options.suffix;   // buf 和 enumerator 會在 finally 釋放

                object currentItem = enumerator.Current;
                bool hasNext = enumerator.MoveNext();

                AppendSpan(ref buf, ref len, options.prefix.AsSpan());

                if (!hasNext)
                {
                    AppendSpan(ref buf, ref len, beginSpan);
                    AppendObj(ref buf, ref len, currentItem, options.tostring, hasFormat);
                    if (!options.exclude_last_end) AppendSpan(ref buf, ref len, endSpan);
                    AppendSpan(ref buf, ref len, options.suffix.AsSpan());
                    return new(buf, 0, len); // finally 會清理 buf
                }

                AppendSpan(ref buf, ref len, beginSpan);
                AppendObj(ref buf, ref len, currentItem, options.tostring, hasFormat);

                while (true)
                {
                    object nextItem = enumerator.Current;
                    hasNext = enumerator.MoveNext();

                    if (!hasNext)
                    {
                        AppendSpan(ref buf, ref len, hasFps ? fpsSpan : endSpan);
                        AppendSpan(ref buf, ref len, beginSpan);
                        AppendObj(ref buf, ref len, nextItem, options.tostring, hasFormat);
                        if (!options.exclude_last_end) AppendSpan(ref buf, ref len, endSpan);
                        break;
                    }

                    AppendSpan(ref buf, ref len, endSpan);
                    AppendSpan(ref buf, ref len, beginSpan);
                    AppendObj(ref buf, ref len, nextItem, options.tostring, hasFormat);
                }

                AppendSpan(ref buf, ref len, options.suffix.AsSpan());
                return new(buf, 0, len);
            }
            finally
            {
                // 釋放資源
                (enumerator as IDisposable)?.Dispose();
                ArrayPool<char>.Shared.Return(buf);
            }
        }
#else
        private static string FE_ProcessEnumerable_Optimized_Legacy(IEnumerable enumerable, in FeOptions options)
        {
            if (enumerable is null) return options.prefix + options.suffix;

            bool hasFps = !string.IsNullOrEmpty(options.final_pair_separator);

            StringBuilder result = new();
            var enumerator = enumerable.GetEnumerator();

            try
            {
                if (!enumerator.MoveNext()) return options.prefix + options.suffix;

                object currentItem = enumerator.Current;
                bool hasNext = enumerator.MoveNext();

                result.Append(options.prefix);

                if (!hasNext)
                {
                    result.Append(options.begin);
                    result.Append(FormatObject(currentItem, options.tostring));
                    if (!options.exclude_last_end) result.Append(options.end);
                    result.Append(options.suffix);
                    return result.ToString();
                }

                // 第一個元素的格式化字串（先不寫入，等迴圈內統一處理）
                string currentStr = FormatObject(currentItem, options.tostring);

                while (hasNext)
                {
                    object nextItem = enumerator.Current;
                    hasNext = enumerator.MoveNext();

                    if (!hasNext)
                    {
                        // 倒數第二個元素：使用 final_pair_separator 或 end
                        result.Append(options.begin).Append(currentStr)
                              .Append(hasFps ? options.final_pair_separator : options.end);

                        // 最後一個元素
                        result.Append(options.begin).Append(FormatObject(nextItem, options.tostring));
                        if (!options.exclude_last_end) result.Append(options.end);
                    }
                    else
                    {
                        // 一般中間元素
                        result.Append(options.begin).Append(currentStr).Append(options.end);
                        currentStr = FormatObject(nextItem, options.tostring);
                    }
                }

                return result.Append(options.suffix).ToString();
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }
#endif
    }
}

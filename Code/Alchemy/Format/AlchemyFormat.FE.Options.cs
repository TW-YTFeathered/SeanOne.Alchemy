// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using SeanOne.Alchemy.Utility;

namespace SeanOne.Alchemy;

partial class Alchemy
{
    private readonly struct FeOptions(
        string begin, string end, string final_pair_separator, string tostring, string dict_format,
        string key_format, string value_format, string prefix, string suffix, bool exclude_last_end, bool fe_opt)
    {
        public readonly string begin = begin;
        public readonly string end = end;
        public readonly string final_pair_separator = final_pair_separator;
        public readonly string tostring = tostring;
        public readonly string dict_format = dict_format;
        public readonly string key_format = key_format;
        public readonly string value_format = value_format;
        public readonly string prefix = prefix;
        public readonly string suffix = suffix;
        public readonly bool exclude_last_end = exclude_last_end;
        public readonly bool fe_opt = fe_opt;

        public static FeOptions Parse(string dsl)
        {
            static string S(string dsl, string p, string d = "") =>
                dsl.ParamField(p, d);
            static bool B(string dsl, string p, bool d = false) =>
                bool.TryParse(S(dsl, p), out var v) ? v : d;

            return new(
                begin: S(dsl, CommonParams.Begin),
                end: S(dsl, CommonParams.End),
                final_pair_separator: S(dsl, IEnumerableParams.FinalPairSeparator),
                tostring: S(dsl, CommonParams.Tostring),
                dict_format: S(dsl, IDictionaryParams.DictFormat),
                key_format: S(dsl, IDictionaryParams.KeyFormat),
                value_format: S(dsl, IDictionaryParams.ValueFormat),
                prefix: S(dsl, CommonParams.Prefix),
                suffix: S(dsl, CommonParams.Suffix),
                exclude_last_end: B(dsl, IEnumerableParams.ExcludeLastEnd),
                fe_opt: B(dsl, FeParams.FeOpt)
            );
        }
    }
}

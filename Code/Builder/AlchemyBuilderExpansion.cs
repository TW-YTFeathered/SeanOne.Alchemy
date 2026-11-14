// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using System;

namespace SeanOne.Alchemy.Builder;

static class AlchemyBuilderExpansion
{
    public static string ToBasicParamString(this BasicParam param) => param switch
    {
        BasicParam.ToString => CommonParams.Tostring,
        BasicParam.Begin => CommonParams.Begin,
        BasicParam.End => CommonParams.End,
        BasicParam.Prefix => CommonParams.Prefix,
        BasicParam.Suffix => CommonParams.Suffix,
        _ => throw new ArgumentOutOfRangeException(nameof(param), param, null),
    };

    public static string ToFeSeqParamString(this FeSeqParam param) => param switch
    {
        FeSeqParam.Begin => CommonParams.Begin,
        FeSeqParam.End => CommonParams.End,
        FeSeqParam.ExcludeLastEnd => IEnumerableParams.ExcludeLastEnd,
        FeSeqParam.ToString => CommonParams.Tostring,
        FeSeqParam.FinalPairSeparator => IEnumerableParams.FinalPairSeparator,
        FeSeqParam.Prefix => CommonParams.Prefix,
        FeSeqParam.Suffix => CommonParams.Suffix,
        FeSeqParam.FeOpt => FeParams.FeOpt,
        _ => throw new ArgumentOutOfRangeException(nameof(param), param, null),
    };

    public static string ToFeDictParamString(this FeDictParam param) => param switch
    {
        FeDictParam.DictFormat => IDictionaryParams.DictFormat,
        FeDictParam.Begin => CommonParams.Begin,
        FeDictParam.End => CommonParams.End,
        FeDictParam.ExcludeLastEnd => IEnumerableParams.ExcludeLastEnd,
        FeDictParam.FinalPairSeparator => IEnumerableParams.FinalPairSeparator,
        FeDictParam.KeyFormat => IDictionaryParams.KeyFormat,
        FeDictParam.ValueFormat => IDictionaryParams.ValueFormat,
        FeDictParam.Prefix => CommonParams.Prefix,
        FeDictParam.Suffix => CommonParams.Suffix,
        FeDictParam.FeOpt => FeParams.FeOpt,
        _ => throw new ArgumentOutOfRangeException(nameof(param), param, null),
    };
}

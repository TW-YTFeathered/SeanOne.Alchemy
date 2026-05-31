// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using System;

namespace SeanOne.Alchemy.Builder
{
    static class AlchemyBuilderExpansion
    {
        public static string ToBasicParamString(this BasicParam param)
        {
            switch (param)
            {
                case BasicParam.ToString:
                    return CommonParams.Tostring;
                case BasicParam.End:
                    return CommonParams.End;
                case BasicParam.Prefix:
                    return CommonParams.Prefix;
                case BasicParam.Suffix:
                    return CommonParams.Suffix;
                default:
                    throw new ArgumentOutOfRangeException(nameof(param), param, null);
            }
        }

        public static string ToFeSeqParamString(this FeSeqParam param)
        {
            switch (param)
            {
                case FeSeqParam.End:
                    return CommonParams.End;
                case FeSeqParam.ExcludeLastEnd:
                    return IEnumerableParams.ExcludeLastEnd;
                case FeSeqParam.ToString:
                    return CommonParams.Tostring;
                case FeSeqParam.FinalPairSeparator:
                    return IEnumerableParams.FinalPairSeparator;
                case FeSeqParam.Prefix:
                    return CommonParams.Prefix;
                case FeSeqParam.Suffix:
                    return CommonParams.Suffix;
                case FeSeqParam.FeOpt:
                    return FeParams.FeOpt;
                default:
                    throw new ArgumentOutOfRangeException(nameof(param), param, null);
            }
        }

        public static string ToFeDictParamString(this FeDictParam param)
        {
            switch (param)
            {
                case FeDictParam.DictFormat:
                    return IDictionaryParams.DictFormat;
                case FeDictParam.End:
                    return CommonParams.End;
                case FeDictParam.ExcludeLastEnd:
                    return IEnumerableParams.ExcludeLastEnd;
                case FeDictParam.FinalPairSeparator:
                    return IEnumerableParams.FinalPairSeparator;
                case FeDictParam.KeyFormat:
                    return IDictionaryParams.KeyFormat;
                case FeDictParam.ValueFormat:
                    return IDictionaryParams.ValueFormat;
                case FeDictParam.Prefix:
                    return CommonParams.Prefix;
                case FeDictParam.Suffix:
                    return CommonParams.Suffix;
                case FeDictParam.FeOpt:
                    return FeParams.FeOpt;
                default:
                    throw new ArgumentOutOfRangeException(nameof(param), param, null);
            }
        }

#if BETA
        public static string ToSortAlgorithmString(this SortAlgorithm param)
        {
            switch (param)
            {
                case SortAlgorithm.Bubble:
                    return "bs";
                case SortAlgorithm.Insertion:
                    return "is";
                case SortAlgorithm.ArraySort:
                    return "as";
                case SortAlgorithm.Linq:
                    return "ls";
                default:
                    throw new ArgumentOutOfRangeException(nameof(param));
            }
        }

        public static string ToDirParamString(this DirParam param)
        {
            switch (param)
            {
                case DirParam.Basic:
                    return "basic";
                case DirParam.FeSeq:
                    return "fe";
                case DirParam.FeDict:
                    return "fe";
                case DirParam.Cnv:
                    return "cnv";
                default:
                    throw new ArgumentOutOfRangeException(nameof(param));
            }
        }

        public static string ToCnvParamString(this CnvParam param)
        {
            switch (param)
            {
                case CnvParam.Sort:
                    return CnvParams.Sort;
                case CnvParam.Temp:
                    return CnvParams.Temp;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
#endif
    }
}

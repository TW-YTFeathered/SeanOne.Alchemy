// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Builder;

public sealed class ArrFunctionBuilder : AlchemyFunctionBuilder<ArrFunctionBuilder, ArrParam>
{
    internal ArrFunctionBuilder(AlchemyBuilder pb) : base(pb, "arr") { }

    protected override string GetParamString(ArrParam param) => param.ToArrParamString();

    public ArrFunctionBuilder WithSort(SortAlgorithm algorithm)
    {
        string value = algorithm.ToSortAlgorithmString();
        return With(ArrParam.Sort, value);
    }
}

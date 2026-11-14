// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Builder;

public sealed class CnvFunctionBuilder : AlchemyFunctionBuilder<CnvFunctionBuilder, CnvParam>
{
    internal CnvFunctionBuilder(AlchemyBuilder pb) : base(pb, "cnv") { }

    protected override string GetParamString(CnvParam param) => param.ToCnvParamString();

    public CnvFunctionBuilder WithTemp(TempUnit from, TempUnit to)
    {
        if (from == to) return this;

        string value = $"{from}->{to}";
        return With(CnvParam.Temp, value);
    }

    public CnvFunctionBuilder WithLength(LengthUnit from, LengthUnit to)
    {
        if (from == to) return this;

        string value = $"{from}->{to}";
        return With(CnvParam.Length, value);
    }

    public CnvFunctionBuilder WithWeight(WeightUnit from, WeightUnit to) 
    {
        if (from == to) return this;

        string value = $"{from}->{to}";
        return With(CnvParam.Weight, value);
    }
}

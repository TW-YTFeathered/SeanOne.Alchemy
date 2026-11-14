namespace SeanOne.Alchemy.Builder;

public sealed class BasicFunctionBuilder :AlchemyFunctionBuilder<BasicFunctionBuilder, BasicParam>
{
    internal BasicFunctionBuilder(AlchemyBuilder pb) : base(pb, "basic") { }

    protected override string GetParamString(BasicParam param) => param.ToBasicParamString();

    public BasicFunctionBuilder WithBegin(string value) => With(BasicParam.Begin, value);
    public BasicFunctionBuilder WithEnd(string value) => With(BasicParam.End, value);
    public BasicFunctionBuilder WithToString(string format) => With(BasicParam.ToString, format);
    public BasicFunctionBuilder WithPrefix(string value) => With(BasicParam.Prefix, value);
    public BasicFunctionBuilder WithSuffix(string value) => With(BasicParam.Suffix, value);
}

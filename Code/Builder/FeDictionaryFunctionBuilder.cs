namespace SeanOne.Alchemy.Builder;

public sealed class FeDictionaryFunctionBuilder : AlchemyFunctionBuilder<FeDictionaryFunctionBuilder, FeDictParam>
{
    internal FeDictionaryFunctionBuilder(AlchemyBuilder pb) : base(pb, "fe") { }

    protected override string GetParamString(FeDictParam param) => param.ToFeDictParamString();

    public FeDictionaryFunctionBuilder WithDictFormat(string format) => With(FeDictParam.DictFormat, format);
    public FeDictionaryFunctionBuilder WithBegin(string value) => With(FeDictParam.Begin, value);
    public FeDictionaryFunctionBuilder WithEnd(string value) => With(FeDictParam.End, value);
    public FeDictionaryFunctionBuilder WithExcludeLastEnd(bool flag = true) => With(FeDictParam.ExcludeLastEnd, flag.ToString().ToLowerInvariant());
    public FeDictionaryFunctionBuilder WithFinalPairSeparator(string value) => With(FeDictParam.FinalPairSeparator, value);
    public FeDictionaryFunctionBuilder WithKeyFormat(string format) => With(FeDictParam.KeyFormat, format);
    public FeDictionaryFunctionBuilder WithValueFormat(string format) => With(FeDictParam.ValueFormat, format);
    public FeDictionaryFunctionBuilder WithPrefix(string value) => With(FeDictParam.Prefix, value);
    public FeDictionaryFunctionBuilder WithSuffix(string value) => With(FeDictParam.Suffix, value);
    public FeDictionaryFunctionBuilder WithFeOpt(bool flag = true) => With(FeDictParam.FeOpt, flag.ToString().ToLowerInvariant());
}

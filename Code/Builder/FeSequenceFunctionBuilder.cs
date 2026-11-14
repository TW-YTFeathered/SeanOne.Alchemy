namespace SeanOne.Alchemy.Builder;

public sealed class FeSequenceFunctionBuilder : AlchemyFunctionBuilder<FeSequenceFunctionBuilder, FeSeqParam>
{
    internal FeSequenceFunctionBuilder(AlchemyBuilder pb) : base(pb, "fe") { }

    protected override string GetParamString(FeSeqParam param) => param.ToFeSeqParamString();

    public FeSequenceFunctionBuilder WithBegin(string value) => With(FeSeqParam.Begin, value);
    public FeSequenceFunctionBuilder WithEnd(string value) => With(FeSeqParam.End, value);
    public FeSequenceFunctionBuilder WithExcludeLastEnd(bool flag = true) => With(FeSeqParam.ExcludeLastEnd, flag.ToString().ToLowerInvariant());
    public FeSequenceFunctionBuilder WithFinalPairSeparator(string value) => With(FeSeqParam.FinalPairSeparator, value);
    public FeSequenceFunctionBuilder WithToString(string format) => With(FeSeqParam.ToString, format);
    public FeSequenceFunctionBuilder WithPrefix(string value) => With(FeSeqParam.Prefix, value);
    public FeSequenceFunctionBuilder WithSuffix(string value) => With(FeSeqParam.Suffix, value);
    public FeSequenceFunctionBuilder WithFeOpt(bool flag = true) => With(FeSeqParam.FeOpt, flag.ToString().ToLowerInvariant());
}

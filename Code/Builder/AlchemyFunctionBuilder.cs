using SeanOne.Alchemy.Definitions;
using System;
using System.Text;

namespace SeanOne.Alchemy.Builder;

public abstract class AlchemyFunctionBuilder<TSelf, TParam>(AlchemyBuilder pb, string cp)
    where TSelf : AlchemyFunctionBuilder<TSelf, TParam>
    where TParam : struct, Enum
{
    protected readonly AlchemyBuilder ParentBuilder = pb ?? throw new ArgumentNullException(nameof(pb));
    protected readonly StringBuilder Sb = new(cp + " ");
    private bool _isStepped;

    protected abstract string GetParamString(TParam param);

    public TSelf With(TParam param, string value)
    {
        if (_isStepped)
            throw new InvalidOperationException("Cannot add parameters after the builder stage has been completed.");

        string paramStr = GetParamString(param);
        string escapedValue = DslSyntaxBuilder.EscapeDslValue(value);

        Sb.AppendParam(paramStr);
        Sb.AppendQuoted(escapedValue);

        return (TSelf)this;
    }

    public AlchemyBuilder Step()
    {
        if (!_isStepped)
        {
            ParentBuilder.AddCommand(Sb.ToString().TrimEnd());
            _isStepped = true;
        }
        return ParentBuilder;
    }

    public BasicFunctionBuilder ThenBasic() => Step().ThenBasic();
    public FeSequenceFunctionBuilder ThenFeSeq() => Step().ThenFeSeq();
    public FeDictionaryFunctionBuilder ThenFeDict() => Step().ThenFeDict();

    public AlchemyExecutable Build() => Step().Build();
    public AlchemyResult Execute(object obj) => Build().Execute(obj);
    public override string ToString() => Build().ToString();
}

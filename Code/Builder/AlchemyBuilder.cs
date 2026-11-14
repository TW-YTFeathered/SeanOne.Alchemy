// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System.Collections.Generic;

namespace SeanOne.Alchemy.Builder;

public class AlchemyBuilder
{
    private readonly List<string> _commands = new();

    private AlchemyBuilder() { }

    public static AlchemyBuilder Create() => new();

    public static BasicFunctionBuilder SelectBasic() => Create().ThenBasic();
    public static FeSequenceFunctionBuilder SelectFeSeq() => Create().ThenFeSeq();
    public static FeDictionaryFunctionBuilder SelectFeDict() => Create().ThenFeDict();

    public BasicFunctionBuilder ThenBasic() => new(this);
    public FeSequenceFunctionBuilder ThenFeSeq() => new(this);
    public FeDictionaryFunctionBuilder ThenFeDict() => new(this);

    internal void AddCommand(string command)
    {
        if (!string.IsNullOrWhiteSpace(command))
            _commands.Add(command);
    }

    public AlchemyExecutable Build() => new(_commands.ToArray());
}

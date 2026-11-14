// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System.Collections.Generic;

namespace SeanOne.Alchemy.Builder;

public class AlchemyBuilder
{
    private readonly List<string> _commands = new();

    private AlchemyBuilder() { }

    public static AlchemyBuilder Create() => new();

    internal void AddCommand(string command)
    {
        if (!string.IsNullOrWhiteSpace(command))
            _commands.Add(command);
    }

    public AlchemyExecutable Build() => new(_commands.ToArray());

    public static BasicFunctionBuilder SelectBasic() => Create().ThenBasic();
    public static FeSequenceFunctionBuilder SelectFeSeq() => Create().ThenFeSeq();
    public static FeDictionaryFunctionBuilder SelectFeDict() => Create().ThenFeDict();
    public static CnvFunctionBuilder SelectCnv() => Create().ThenCnv();
    public static ArrFunctionBuilder SelectArr() => Create().ThenArr();
}

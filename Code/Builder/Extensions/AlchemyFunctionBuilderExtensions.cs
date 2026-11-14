// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;

namespace SeanOne.Alchemy.Builder;

public static class AlchemyFunctionBuilderExtensions
{
    //extension(AlchemyFunctionBuilder afb)
    extension<TSelf, TParam>(AlchemyFunctionBuilder<TSelf, TParam> fb)
        where TSelf : AlchemyFunctionBuilder<TSelf, TParam>
        where TParam : struct, Enum
    {
        public BasicFunctionBuilder ThenBasic() => fb.Step().ThenBasic();
        public FeDictionaryFunctionBuilder ThenFeDict() => fb.Step().ThenFeDict();
        public FeSequenceFunctionBuilder ThenFeSeq() => fb.Step().ThenFeSeq();
        public CnvFunctionBuilder ThenCnv() => fb.Step().ThenCnv();
        public ArrFunctionBuilder ThenArr() => fb.Step().ThenArr();
    }
}

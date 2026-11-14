// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Builder;

public static class AlchemyBuilderExtensions
{
    extension(AlchemyBuilder pb)
    {
        public BasicFunctionBuilder ThenBasic() => new(pb);
        public FeDictionaryFunctionBuilder ThenFeDict() => new(pb);
        public FeSequenceFunctionBuilder ThenFeSeq() => new(pb);
    }
}

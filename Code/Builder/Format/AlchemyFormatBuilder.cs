// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Builder
{
    /// <summary>
    /// Provides methods to initialize and build DSL functions.
    /// </summary>
    public class AlchemyFormatBuilder
    {
        /// <summary>
        /// Creates an instance of the <see cref="BasicFunc"/> class.
        /// </summary>
        /// <returns>An <see cref="IBasicAlchemyFunction{BasicParam}"/> implementation.</returns>
        public static IBasicAlchemyFunction<BasicParam> SelectBasic() => new BasicFunc();

        /// <summary>
        /// Creates an instance of the <see cref="FeSequenceFunc"/> class.
        /// Note: Internally relies on the same underlying DSL function (fe) as <see cref="FeDictionaryFunc"/>,
        /// but provided here for semantic distinction.
        /// </summary>
        /// <returns>An <see cref="ISequenceAlchemyFunction{FeSeqParam}"/> implementation.</returns>
        public static ISequenceAlchemyFunction<FeSeqParam> SelectFeSeq() => new FeSequenceFunc();

        /// <summary>
        /// Creates an instance of the <see cref="FeDictionaryFunc"/> class.
        /// Note: Internally relies on the same underlying DSL function (fe) as <see cref="FeSequenceFunc"/>,
        /// but provided here for semantic distinction.
        /// </summary>
        /// <returns>An <see cref="IDictionaryAlchemyFunction{FeDictParam}"/> implementation.</returns>
        public static IDictionaryAlchemyFunction<FeDictParam> SelectFeDict() => new FeDictionaryFunc();

        public static CnvFunc SelectCnv() => new CnvFunc();

        public static AlchemyConversionBuilder CreatePipeline() => new AlchemyConversionBuilder();
    }
}

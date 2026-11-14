// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;

namespace SeanOne.Alchemy.Builder
{
    /// <summary>
    /// Provides methods to initialize and build DSL functions.
    /// </summary>
    [Obsolete("AlchemyFormatBuilder is deprecated and will be removed in a future version. Use AlchemyBuilder instead.")]
    public class AlchemyFormatBuilder
    {
        /// <summary>
        /// Creates an instance of the <see cref="BasicFunc"/> class.
        /// </summary>
        /// <returns>An <see cref="IBasicAlchemyFunction{BasicParam}"/> implementation.</returns>
        [Obsolete("This method is deprecated and will be removed in a future version. Use AlchemyBuilder.SelectBasic() instead.")]
        public static IBasicAlchemyFunction<BasicParam> SelectBasic() => new BasicFunc();

        /// <summary>
        /// Creates an instance of the <see cref="FeSequenceFunc"/> class.
        /// Note: Internally relies on the same underlying DSL function (fe) as <see cref="FeDictionaryFunc"/>,
        /// but provided here for semantic distinction.
        /// </summary>
        /// <returns>An <see cref="ISequenceAlchemyFunction{FeSeqParam}"/> implementation.</returns>
        [Obsolete("This method is deprecated and will be removed in a future version. Use AlchemyBuilder.SelectFeSeq() instead.")]
        public static ISequenceAlchemyFunction<FeSeqParam> SelectFeSeq() => new FeSequenceFunc();

        /// <summary>
        /// Creates an instance of the <see cref="FeDictionaryFunc"/> class.
        /// Note: Internally relies on the same underlying DSL function (fe) as <see cref="FeSequenceFunc"/>,
        /// but provided here for semantic distinction.
        /// </summary>
        /// <returns>An <see cref="IDictionaryAlchemyFunction{FeDictParam}"/> implementation.</returns>
        [Obsolete("This method is deprecated and will be removed in a future version. Use AlchemyBuilder.SelectFeDict() instead.")]
        public static IDictionaryAlchemyFunction<FeDictParam> SelectFeDict() => new FeDictionaryFunc();
    }
}

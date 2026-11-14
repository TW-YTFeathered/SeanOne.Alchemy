// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;

namespace SeanOne.Alchemy.Builder
{
    /// <summary>
    /// Generic base interface for DSL functions.
    /// Provides methods to configure parameters and build a DSL executable.
    /// </summary>
    /// <typeparam name="TParam">The type of parameter used in the DSL function.</typeparam>
    [Obsolete("IAlchemyFunction is deprecated and will be restructured in a future version. Use AlchemyBuilder factory methods instead.")]
    public interface IAlchemyFunction<TParam>
    {
        /// <summary>
        /// Adds a parameter and its associated value to the DSL function.
        /// </summary>
        /// <param name="param">The parameter to configure.</param>
        /// <param name="value">The value associated with the parameter.</param>
        /// <returns>The current DSL function instance for chaining.</returns>
        [Obsolete("This method is part of the legacy builder API. Use modern AlchemyBuilder fluent methods instead.")]
        IAlchemyFunction<TParam> With(TParam param, string value);

        /// <summary>
        /// Builds the DSL function into an executable instance.
        /// </summary>
        /// <returns>A <see cref="AlchemyExecutable"/> representing the configured DSL function.</returns>
        AlchemyExecutable Build();
    }

    /// <summary>
    /// DSL function interface for basic operations.
    /// Inherits from <see cref="IAlchemyFunction{TParam}"/>.
    /// </summary>
    /// <typeparam name="TParam">The type of parameter used in the DSL function.</typeparam>
    [Obsolete("IBasicAlchemyFunction is deprecated and will be removed in a future version. Use AlchemyBuilder.SelectBasic() instead.")]
    public interface IBasicAlchemyFunction<TParam> : IAlchemyFunction<TParam> { }
    /// <summary>
    /// DSL function interface for sequence-based operations.
    /// Note: Internally relies on the same underlying DSL function <c>fe</c>
    /// as <see cref="IDictionaryAlchemyFunction{TParam}"/>, but provided here
    /// for semantic distinction.
    /// </summary>
    /// <typeparam name="TParam">The type of parameter used in the DSL function.</typeparam>
    [Obsolete("ISequenceAlchemyFunction is deprecated and will be removed in a future version. Use AlchemyBuilder.SelectFeSeq() instead.")]
    public interface ISequenceAlchemyFunction<TParam> : IAlchemyFunction<TParam> { }
    /// <summary>
    /// DSL function interface for dictionary-based operations.
    /// Note: Internally relies on the same underlying DSL function <c>fe</c>
    /// as <see cref="ISequenceAlchemyFunction{TParam}"/>, but provided here
    /// for semantic distinction.
    /// </summary>
    /// <typeparam name="TParam">The type of parameter used in the DSL function.</typeparam>
    [Obsolete("IDictionaryAlchemyFunction is deprecated and will be removed in a future version. Use AlchemyBuilder.SelectFeDict() instead.")] 
    public interface IDictionaryAlchemyFunction<TParam> : IAlchemyFunction<TParam> { }
}

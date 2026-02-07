// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Builder
{
    /// <summary>
    /// DSL executable class.
    /// Provides an interface to run DSL instructions and retrieve the DSL string.
    /// </summary>
    public class AlchemyExecutable
    {
        private readonly string _dsl;

        /// <summary>
        /// 初始化 DSL 執行器
        /// </summary>
        /// <param name="dsl"> 要傳入的 DSL 指令 </param>
        internal AlchemyExecutable(string dsl)
        {
            _dsl = dsl;
        }

        /// <summary>
        /// Run DSL instruction.
        /// </summary>
        /// <param name="obj">Format object.</param>
        /// <returns>DSL formatter result.</returns>
        public string Run(object obj)
        {
            return AlchemyFormatter.Format(obj, _dsl);
        }

        /// <summary>
        /// Get DSL string.
        /// </summary>
        /// <returns>DSL string.</returns>
        public override string ToString() => _dsl;
    }

    /// <summary>
    /// Implements the BuildRun extension method. 
    /// Note: Future DSL syntactic sugar methods will also be placed here.
    /// </summary>
    public static class AlchemyFunctionExtensions
    {
        /// <summary>
        /// Builds the DSL formatter and immediately executes it with the specified object.
        /// Equivalent to calling <c>Build().Run(obj)</c>.
        /// </summary>
        /// <typeparam name="TParam">The type of parameter enum used by the DSL function.</typeparam>
        /// <param name="func">The DSL function builder instance.</param>
        /// <param name="obj">The object to be formatted.</param>
        /// <returns>The formatted result string.</returns>
        public static string BuildRun<TParam>(this IAlchemyFunction<TParam> func, object obj)
            => func.Build().Run(obj);

        /// <summary>
        /// Builds the DSL formatter and returns its DSL string representation.
        /// Equivalent to calling <c>Build().ToString()</c>.
        /// </summary>
        /// <typeparam name="TParam">The type of parameter used in the DSL function.</typeparam>
        /// <param name="func">The DSL function builder instance.</param>
        /// <returns>The DSL string representing the configured formatting rules.</returns>
        public static string BuildToString<TParam>(this IAlchemyFunction<TParam> func)
            => func.Build().ToString();
    }
}

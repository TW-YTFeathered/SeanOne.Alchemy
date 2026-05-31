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
        private readonly string[] _dsls;

        /// <summary>
        /// 取得 _dsls 的 value
        /// </summary>
        internal string[] GetDsls() => _dsls;

        /// <summary>
        /// 初始化 DSL 執行器
        /// </summary>
        /// <param name="dsls"> 要傳入的 DSL 指令 </param>
        internal AlchemyExecutable(params string[] dsls)
        {
            _dsls = dsls;
        }

        /// <summary>
        /// Runs the DSL instruction and returns the formatted result.
        /// </summary>
        /// <remarks>
        /// <para><b>Note:</b> For compatibility with legacy code, this method only processes 
        /// the first instruction in the pipeline. If the executable contains multiple instructions 
        /// (e.g., built with AlchemyConversionBuilder - Beta only), use RunAsConvert (Beta) 
        /// instead to execute the full pipeline.</para>
        /// </remarks>
        public string Run(object obj)
        {
            return AlchemyFormatter.Format(obj, _dsls[0]);
        }

#if BETA
        /// <summary>
        /// Executes the full DSL pipeline by converting and formatting the input object.
        /// This method invokes the core conversion engine to process all instructions.
        /// </summary>
        /// <param name="obj">The source object to be processed.</param>
        /// <returns>The result of the conversion and formatting pipeline.</returns>
        public AlchemyResult RunAsConvert(object obj)
        {
            return AlchemyConverter.Convert(obj, _dsls);
        }
#endif

        /// <summary>
        /// Get DSL string.
        /// </summary>
        /// <returns>DSL string.</returns>
        public override string ToString() => string.Join(" ", _dsls);
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

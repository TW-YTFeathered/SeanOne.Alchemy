using System;
using System.Text;

namespace SeanOne.Alchemy.Builder
{
    /// <summary>
    /// Provides methods to initialize and build DSL functions.
    /// </summary>
    public class AlchemyFormatBuilder
    {
        #region Generic Factory (Commented Out)
        /*
        /// <summary>
        /// Represents the available function types.
        /// Note: FeSequence and FeDictionary are distinguished at the enum level,
        /// but both rely on the same underlying DSL function (fe).
        /// </summary>
        public enum FunctionName
        {
            /// <summary>
            /// Sequence-based DSL function (internally uses the same 'fe' function).
            /// </summary>
            FeSequence,

            /// <summary>
            /// Dictionary-based DSL function (internally uses the same 'fe' function).
            /// </summary>
            FeDictionary,

            /// <summary>
            /// Basic DSL function.
            /// </summary>
            Basic
        }
        */

        /*
        /// <summary>
        /// Generic factory method to select a DSL function based on the specified function name.
        /// </summary>
        /// <typeparam name="TParam">The type of parameter used by the DSL function.</typeparam>
        /// <param name="funcName">The function type to select.</param>
        /// <returns>An implementation of <see cref="IAlchemyFunction{TParam}"/>.</returns>
        public static IAlchemyFunction<TParam> SelectFunction<TParam>(FunctionName funcName)
        {
            switch (funcName)
            {
                case FunctionName.FeSequence:
                    return (IAlchemyFunction<TParam>)new FeSequenceFunc();
                case FunctionName.FeDictionary:
                    return (IAlchemyFunction<TParam>)new FeDictionaryFunc();
                case FunctionName.Basic:
                    return (IAlchemyFunction<TParam>)new BasicFunc();
                default:
                    throw new ArgumentOutOfRangeException(nameof(funcName), funcName, null);
            }
        }
        */
        #endregion

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
    }

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

    #region Generics
    /// <summary>
    /// Generic base interface for DSL functions.
    /// Provides methods to configure parameters and build a DSL executable.
    /// </summary>
    /// <typeparam name="TParam">The type of parameter used in the DSL function.</typeparam>
    public interface IAlchemyFunction<TParam>
    {
        /// <summary>
        /// Adds a parameter and its associated value to the DSL function.
        /// </summary>
        /// <param name="param">The parameter to configure.</param>
        /// <param name="value">The value associated with the parameter.</param>
        /// <returns>The current DSL function instance for chaining.</returns>
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
    public interface IBasicAlchemyFunction<TParam> : IAlchemyFunction<TParam> { }
    /// <summary>
    /// DSL function interface for sequence-based operations.
    /// Note: Internally relies on the same underlying DSL function <c>fe</c>
    /// as <see cref="IDictionaryAlchemyFunction{TParam}"/>, but provided here
    /// for semantic distinction.
    /// </summary>
    /// <typeparam name="TParam">The type of parameter used in the DSL function.</typeparam>
    public interface ISequenceAlchemyFunction<TParam> : IAlchemyFunction<TParam> { }
    /// <summary>
    /// DSL function interface for dictionary-based operations.
    /// Note: Internally relies on the same underlying DSL function <c>fe</c>
    /// as <see cref="ISequenceAlchemyFunction{TParam}"/>, but provided here
    /// for semantic distinction.
    /// </summary>
    /// <typeparam name="TParam">The type of parameter used in the DSL function.</typeparam>
    public interface IDictionaryAlchemyFunction<TParam> : IAlchemyFunction<TParam> { }
    #endregion

    /// <summary>
    /// Implements the BuildRun extension method. 
    /// Note: Future DSL syntactic sugar methods will also be placed here.
    /// </summary>
    public static class AlchemyFunctionExtensions
    {
        /// <summary>
        /// Builds the DSL function and immediately executes it with the specified object.
        /// Equivalent to calling <c>Build().Run(obj)</c>.
        /// </summary>
        /// <typeparam name="TParam">The type of parameter used in the DSL function.</typeparam>
        /// <param name="func">The DSL function instance to be executed.</param>
        /// <param name="obj">The object to be formatted.</param>
        /// <returns>The formatted DSL result string.</returns>
        public static string BuildRun<TParam>(this IAlchemyFunction<TParam> func, object obj)
            => func.Build().Run(obj);
    }

    #region Fe Method
    /// <summary>
    /// Defines the parameter options used by <c>ISequenceAlchemyFunction</c> (fe).
    /// Handles sequence-specific DSL parameters and formatting logic.
    /// </summary>
    public enum FeSeqParam
    {
        /// <summary>
        /// Appends a string after each value.
        /// DSL param: <c>end</c>.
        /// </summary>
        End,

        /// <summary>
        /// Omits the end string after the last item.
        /// DSL param: <c>exclude-last-end</c>.
        /// </summary>
        ExcludeLastEnd,

        /// <summary>
        /// Replaces the separator between the last two items in the sequence. Falls back to <c>end</c> if not specified.
        /// DSL param: <c>final-pair-separator</c>.
        /// </summary>
        FinalPairSeparator,

        /// <summary>
        /// Applies formatting to items implementing <c>IFormattable</c>. Not applicable to dictionaries. Use C#'s <c>ToString()</c> method.
        /// DSL param: <c>tostring</c>.
        /// </summary>
        ToString
    }
    /// <summary>
    /// Implementation of <c>ISequenceAlchemyFunction</c> using <c>FeSeqParam</c>.
    /// </summary>
    public class FeSequenceFunc : ISequenceAlchemyFunction<FeSeqParam>
    {
        // 暫存的字串
        private readonly StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// 初始化，先添加 fe
        /// </summary>
        internal FeSequenceFunc()
        {
            _sb.Append("fe ");
        }

        /// <summary>
        /// Adds a parameter and its associated value to the DSL function.
        /// </summary>
        /// <param name="param">The DSL parameter to configure.</param>
        /// <param name="value">The value associated with the parameter.</param>
        /// <returns>The current DSL function instance for chaining.</returns>
        public IAlchemyFunction<FeSeqParam> With(FeSeqParam param, string value)
        {
            value = DslSyntaxBuilder.EscapeDslValue(value);

            switch (param)
            {
                case FeSeqParam.End:
                    _sb.AppendParam("end").AppendQuoted(value);
                    break;
                case FeSeqParam.ExcludeLastEnd:
                    _sb.AppendParam("exclude-last-end").AppendQuoted(value);
                    break;
                case FeSeqParam.ToString:
                    _sb.AppendParam("tostring").AppendQuoted(value);
                    break;
                case FeSeqParam.FinalPairSeparator:
                    _sb.AppendParam("final-pair-separator").AppendQuoted(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(param), param, null);
            }
            return this; // 回傳自己，支援 Fluent DSL
        }

        /// <summary>
        /// Builds the DSL function into a <see cref="AlchemyExecutable"/> instance.
        /// </summary>
        /// <returns>The constructed <see cref="AlchemyExecutable"/>.</returns>
        public AlchemyExecutable Build()
        {
            return new AlchemyExecutable(_sb.ToString());
        }
    }

    /// <summary>
    /// Defines the parameter options used by <c>IDictionaryAlchemyFunction</c> (fe).
    /// Handles dictionary-specific DSL parameters and formatting logic.
    /// </summary>
    public enum FeDictParam
    {
        /// <summary>
        /// Format string for dictionary entries. 
        /// Use <c>{0}</c> to represent the key and <c>{1}</c> to represent the value.
        /// DSL param: <c>dict-format</c>.
        /// </summary>
        DictFormat,

        /// <summary>
        /// Appends a string after each value.
        /// DSL param: <c>end</c>.
        /// </summary>
        End,

        /// <summary>
        /// Omits the end string after the last item.
        /// DSL param: <c>exclude-last-end</c>.
        /// </summary>
        ExcludeLastEnd,

        /// <summary>
        /// Replaces the separator between the last two items in the sequence. Falls back to `end` if not specified.
        /// DSL param: <c>final-pair-separator</c>.
        /// </summary>
        FinalPairSeparator,

        /// <summary>
        /// Format string applied to dictionary keys.
        /// DSL param: <c>key-format</c>.
        /// </summary>
        KeyFormat,

        /// <summary>
        /// Format string applied to dictionary values.
        /// DSL param: <c>value-format</c>.
        /// </summary>
        ValueFormat
    }
    /// <summary>
    /// Implementation of <c>IDictionaryAlchemyFunction</c> using <c>FeDictParam</c>.
    /// Handles dictionary-specific DSL parameters and formatting logic.
    /// </summary>
    public class FeDictionaryFunc : IDictionaryAlchemyFunction<FeDictParam>
    {
        // 暫存的字串
        private readonly StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// 初始化，先添加 fe
        /// </summary>
        internal FeDictionaryFunc()
        {
            _sb.Append("fe ");
        }

        /// <summary>
        /// Adds a parameter and its associated value to the DSL function.
        /// </summary>
        /// <param name="param">The DSL parameter to configure.</param>
        /// <param name="value">The value associated with the parameter.</param>
        /// <returns>The current DSL function instance for chaining.</returns>
        public IAlchemyFunction<FeDictParam> With(FeDictParam param, string value)
        {
            value = DslSyntaxBuilder.EscapeDslValue(value);

            switch (param)
            {
                case FeDictParam.DictFormat:
                    _sb.AppendParam("dict-format").AppendQuoted(value);
                    break;
                case FeDictParam.End:
                    _sb.AppendParam("end").AppendQuoted(value);
                    break;
                case FeDictParam.ExcludeLastEnd:
                    _sb.AppendParam("exclude-last-end").AppendQuoted(value);
                    break;
                case FeDictParam.FinalPairSeparator:
                    _sb.AppendParam("final-pair-separator").AppendQuoted(value);
                    break;
                case FeDictParam.KeyFormat:
                    _sb.AppendParam("key-format").AppendQuoted(value);
                    break;
                case FeDictParam.ValueFormat:
                    _sb.AppendParam("value-format").AppendQuoted(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(param), param, null);
            }
            return this; // 回傳自己，支援 Fluent DSL
        }

        /// <summary>
        /// Builds the DSL function into a <see cref="AlchemyExecutable"/> instance.
        /// </summary>
        /// <returns>The constructed <see cref="AlchemyExecutable"/>.</returns>
        public AlchemyExecutable Build()
        {
            return new AlchemyExecutable(_sb.ToString());
        }
    }
    #endregion

    #region Basic Method
    /// <summary>
    /// Defines the parameter options used by <c>IBasicAlchemyFunction</c> (basic).
    /// Handles basic DSL parameters and formatting logic.
    /// </summary>
    public enum BasicParam 
    {
        /// <summary>
        /// Appends a string after each value.
        /// DSL param: <c>end</c>.
        /// </summary>
        End,

        /// <summary>
        /// Applies formatting to items implementing <c>IFormattable</c>. Not applicable to dictionaries. Use C#'s <c>ToString()</c> method.
        /// DSL param: <c>tostring</c>.
        /// </summary>
        ToString
    }
    /// <summary>
    /// Implementation of <c>IBasicAlchemyFunction</c> using <c>BasicParam</c>.
    /// Handles basic DSL parameters and formatting logic.
    /// </summary>
    public class BasicFunc : IBasicAlchemyFunction<BasicParam>
    {
        // 暫存的字串
        private readonly StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// 初始化，先添加 basic
        /// </summary>
        internal BasicFunc()
        {
            _sb.Append("basic ");
        }

        /// <summary>
        /// Adds a parameter and its associated value to the DSL function.
        /// </summary>
        /// <param name="param">The DSL parameter to configure.</param>
        /// <param name="value">The value associated with the parameter.</param>
        /// <returns>The current DSL function instance for chaining.</returns>
        public IAlchemyFunction<BasicParam> With(BasicParam param, string value)
        {
            value = DslSyntaxBuilder.EscapeDslValue(value);

            switch (param)
            {
                case BasicParam.ToString:
                    _sb.AppendParam("tostring").AppendQuoted(value);
                    break;
                case BasicParam.End:
                    _sb.AppendParam("end").AppendQuoted(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(param), param, null);
            }
            return this; // 回傳自己，支援 Fluent DSL
        }

        /// <summary>
        /// Builds the DSL function into a <see cref="AlchemyExecutable"/> instance.
        /// </summary>
        /// <returns>The constructed <see cref="AlchemyExecutable"/>.</returns>
        public AlchemyExecutable Build()
        {
            return new AlchemyExecutable(_sb.ToString());
        }
    }
    #endregion
}

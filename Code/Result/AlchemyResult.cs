// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeanOne.Alchemy
{
    public partial class AlchemyResult
    {
        // 儲存原始的物件
        internal readonly object _source;

        // 取得原始的物件
        protected AlchemyResult(object source)
        {
            _source = source;
        }

        // 轉成 List<T>
        public List<T> ToList<T>()
        {
            // 如果對方是集合，直接轉型
            if (_source is IEnumerable<T> enumerable)
            {
                return enumerable.ToList();
            }
            return new List<T>();
        }

        // 轉成指定型別
        public T ToObject<T>()
        {
            return (T)_source;
        }

        // 轉為 string
        public override string ToString()
        {
            // 如果原始物件是 string，直接回傳
            if (_source is string str)
            {
                return str;
            }

            return base.ToString();
        }

        // 非同步 ToString
        public async Task<string> ToStringAsync()
        {
            // 簡單情況下直接返回同步版本
            return await Task.Run(() => { 
                return this.ToString();
            });
        }

        #region Convert/ConvertAsync
        /// <summary>
        /// Converts the current source object according to the provided DSL instruction.
        /// </summary>
        /// <param name="dslInstruction">The DSL instruction string.</param>
        /// <returns>An <see cref="AlchemyResult"/> representing the converted object.</returns>
        public AlchemyResult Convert(string dslInstruction)
        {
            return AlchemyConverter.Convert(_source, dslInstruction);
        }

        public AlchemyResult Convert(params string[] dslInstructions)
        {
            return AlchemyConverter.Convert(_source, dslInstructions);
        }

        /// <summary>
        /// Asynchronously converts the specified object according to the provided DSL instruction.
        /// </summary>
        /// <param name="dslInstruction">The DSL instruction string.</param>
        /// <returns>An <see cref="AlchemyResult"/> representing the converted object.</returns>
        public Task<AlchemyResult> ConvertAsync(string dslInstruction)
        {
            return AlchemyConverter.ConvertAsync(_source, dslInstruction);
        }

        public Task<AlchemyResult> ConvertAsync(params string[] dslInstructions)
        {
            return AlchemyConverter.ConvertAsync(_source, dslInstructions);
        }
        #endregion

        #region Parse/TryParse
        /// <summary>
        /// Creates a new instance of the AlchemyResult class from the specified object.
        /// </summary>
        /// <param name="sourceObj">The source object to wrap in an AlchemyResult.</param>
        /// <returns>A new AlchemyResult instance that represents the specified object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sourceObj"/> is null.</exception>
        public static AlchemyResult Parse(object sourceObj)
        {
            if (sourceObj is null)
            {
                throw new ArgumentNullException(nameof(sourceObj), "AlchemyResult.Parse requires a non-null source object.");
            }

            return new AlchemyResult(sourceObj);
        }

        /// <summary>
        /// Attempts to convert the specified object to an AlchemyResult without throwing an exception.
        /// </summary>
        /// <param name="sourceObj">The source object to wrap in an AlchemyResult. This parameter can be null.</param>
        /// <param name="result">When this method returns, contains the AlchemyResult representation of the object if the conversion succeeded; otherwise, null.</param>
        /// <returns>true if the object was successfully converted to an AlchemyResult; otherwise, false.</returns>
        public static bool TryParse(object sourceObj, out AlchemyResult result)
        {
            if (sourceObj == null)
            {
                result = null; // 失敗時設為 null
                return false;
            }

            result = new AlchemyResult(sourceObj);
            return true;
        }
        #endregion
    }
}

// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeanOne.Alchemy
{
    /// <summary>
    /// Represents the result of an alchemy transformation, wrapping a source object and providing methods to convert, transform, and access the underlying data.
    /// </summary>
    public class AlchemyResult
    {
        // 儲存原始的物件
        private readonly object _source;

        /// <summary>
        /// Gets the raw source object that is wrapped by this <see cref="AlchemyResult"/>.
        /// </summary>
        /// <value>The original object passed to the constructor.</value>
        public object RawSource => _source;  // 唯讀屬性

        // 包裝原始的物件
        private AlchemyResult(object source)
        {
            _source = source;
        }

        /// <summary>
        /// Converts the wrapped source object to a <see cref="List{T}"/> if it is an enumerable sequence of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <returns>A <see cref="List{T}"/> containing the elements of the source sequence.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the wrapped source object is not an <see cref="IEnumerable{T}"/>.
        /// </exception>
        public List<T> ToList<T>()
        {
            // 如果對方是集合，直接轉型
            if (_source is IEnumerable<T> enumerable)
                return enumerable.ToList();
            throw new InvalidOperationException($"The source object of type '{_source?.GetType().Name ?? "null"}' cannot be converted to List<{typeof(T).Name}>.");
        }

        /// <summary>
        /// Casts the wrapped source object to the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The target type to cast to.</typeparam>
        /// <returns>The source object cast to <typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// Thrown if the source object cannot be cast to <typeparamref name="T"/>.
        /// </exception>
        public T ToObject<T>() => (T)_source;

        /// <summary>
        /// Returns a string representation of the wrapped source object.
        /// If the source is already a string, it is returned directly.
        /// Otherwise, the base <see cref="object.ToString"/> implementation is used.
        /// </summary>
        /// <returns>A string that represents the wrapped source object.</returns>
        public override string ToString()
        {
            // 如果原始物件是 string，直接回傳
            if (_source is string str)
                return str;
            return base.ToString();
        }

        /// <summary>
        /// Asynchronously returns a string representation of the wrapped source object.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the string representation of the source object.
        /// </returns>
        /// <remarks>
        /// This method simply offloads <see cref="ToString"/> to a thread-pool thread using
        /// <see cref="Task.Run"/>; it does not perform true asynchronous I/O.
        /// </remarks>
        public async Task<string> ToStringAsync()
        {
            // 簡單情況下直接返回同步版本
            return await Task.Run(() =>
            {
                return this.ToString();
            });
        }

        #region Transform/TransformAsync
        /// <summary>
        /// Transforms the current source object according to the provided DSL instruction.
        /// </summary>
        /// <param name="dslInstruction">The DSL instruction string.</param>
        /// <returns>An <see cref="AlchemyResult"/> representing the transformed object.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dslInstruction"/> is <c>null</c> or empty.
        /// </exception>
        /// <seealso cref="Alchemy.Transform(object, string)"/>
        public AlchemyResult Transform(string dslInstruction) =>
            Alchemy.Transform(_source, dslInstruction);

        /// <summary>
        /// Transforms the current source object by sequentially applying multiple DSL instructions.
        /// </summary>
        /// <param name="dslInstructions">An array of DSL instruction strings to apply in order.</param>
        /// <returns>An <see cref="AlchemyResult"/> representing the final transformed object.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dslInstructions"/> is <c>null</c> or empty.
        /// </exception>
        /// <seealso cref="Alchemy.Transform(object, string[])"/>
        public AlchemyResult Transform(params string[] dslInstructions) =>
            Alchemy.Transform(_source, dslInstructions);

        /// <summary>
        /// Asynchronously transforms the current source object according to the provided DSL instruction.
        /// </summary>
        /// <param name="dslInstruction">The DSL instruction string.</param>
        /// <returns>
        /// A task that represents the asynchronous transformation operation.
        /// The task result contains an <see cref="AlchemyResult"/> representing the transformed object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dslInstruction"/> is <c>null</c> or empty.
        /// </exception>
        /// <seealso cref="Alchemy.TransformAsync(object, string)"/>
        public Task<AlchemyResult> TransformAsync(string dslInstruction) =>
            Alchemy.TransformAsync(_source, dslInstruction);

        /// <summary>
        /// Asynchronously transforms the current source object by sequentially applying multiple DSL instructions.
        /// </summary>
        /// <param name="dslInstructions">An array of DSL instruction strings to apply in order.</param>
        /// <returns>
        /// A task that represents the asynchronous transformation operation.
        /// The task result contains an <see cref="AlchemyResult"/> representing the final transformed object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dslInstructions"/> is <c>null</c> or empty.
        /// </exception>
        /// <seealso cref="Alchemy.TransformAsync(object, string[])"/>
        public Task<AlchemyResult> TransformAsync(params string[] dslInstructions) =>
            Alchemy.TransformAsync(_source, dslInstructions);
        #endregion

        #region Parse/TryParse
        /// <summary>
        /// Creates a new <see cref="AlchemyResult"/> instance that wraps the specified source object.
        /// </summary>
        /// <param name="sourceObj">The source object to wrap. Must not be <c>null</c>.</param>
        /// <returns>A new <see cref="AlchemyResult"/> that contains <paramref name="sourceObj"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="sourceObj"/> is <c>null</c>.
        /// </exception>
        public static AlchemyResult Parse(object sourceObj)
        {
            if (sourceObj is null)
                 throw new ArgumentNullException(nameof(sourceObj), "AlchemyResult.Parse requires a non-null source object.");
            return new AlchemyResult(sourceObj);
        }

        /// <summary>
        /// Attempts to wrap the specified source object in an <see cref="AlchemyResult"/>.
        /// </summary>
        /// <param name="sourceObj">The source object to wrap. May be <c>null</c>.</param>
        /// <param name="result">
        /// When this method returns, contains the <see cref="AlchemyResult"/> that wraps <paramref name="sourceObj"/>
        /// if the operation succeeded; otherwise, <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="sourceObj"/> is not <c>null</c> and the wrapping succeeded;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool TryParse(object sourceObj, out AlchemyResult result)
        {
            if (sourceObj is null)
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

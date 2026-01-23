using System;
using System.Collections.Generic;
using System.Linq;

namespace SeanOne.Alchemy
{
    public class AlchemyResult
    {
        // 儲存原始的物件
        private readonly object _source;

        // 取得原始的物件
        private AlchemyResult(object source)
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
            return new List<T> { (T)_source };
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

            try
            {
                result = Parse(sourceObj); // 這裡可能拋錯
                return true;
            }
            catch
            {
                result = null; // 失敗時設為 null
                return false;
            }
        }
        #endregion
    }
}

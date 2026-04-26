// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Definitions
{
    /// <summary>
    /// 提供通用 DSL 參數名稱常數，適用於多種指令與格式化情境
    /// </summary>
    internal static class CommonParams
    {
        /// <summary>
        /// 每個值之前附加的字串
        /// DSL 參數: <c>begin</c>
        /// </summary>
        public const string Begin = "begin";

        /// <summary>
        /// 每個值之後附加的字串
        /// DSL 參數: <c>end</c>
        /// </summary>
        public const string End = "end";

        /// <summary>
        /// 套用於實作 <c>IFormattable</c> 之物件的格式字串
        /// DSL 參數: <c>tostring</c>
        /// </summary>
        public const string Tostring = "tostring";

        /// <summary>
        /// 整個輸出結果之前附加的前綴字串
        /// DSL 參數: <c>prefix</c>
        /// </summary>
        public const string Prefix = "prefix";

        /// <summary>
        /// 整個輸出結果之後附加的後綴字串
        /// DSL 參數: <c>suffix</c>
        /// </summary>
        public const string Suffix = "suffix";
    }

    /// <summary>
    /// 提供 FE 指令專用的參數名稱常數
    /// </summary>
    internal static class FeParams
    {
        /// <summary>
        /// 啟用最佳化格式化模式 (可能影響相容性)
        /// DSL 參數: <c>fe-opt</c>
        /// </summary>
        public const string FeOpt = "fe-opt";
    }

    /// <summary>
    /// 提供與 <see cref="System.Collections.IEnumerable"/> 處理相關的 DSL 參數名稱常數
    /// </summary>
    internal static class IEnumerableParams
    {
        /// <summary>
        /// 是否排除最後一個項目的 <c>end</c> 字串
        /// DSL 參數: <c>exclude-last-end</c>
        /// </summary>
        public const string ExcludeLastEnd = "exclude-last-end";

        /// <summary>
        /// 用於取代最後兩個項目之間分隔符號的字串 (若未指定則使用 <c>end</c>)
        /// DSL 參數: <c>final-pair-separator</c>
        /// </summary>
        public const string FinalPairSeparator = "final-pair-separator";
    }

    /// <summary>
    /// 提供與 <see cref="System.Collections.IDictionary"/> 處理相關的 DSL 參數名稱常數
    /// </summary>
    internal static class IDictionaryParams
    {
        /// <summary>
        /// 定義字典項目的格式字串，可使用 <c>{0}</c> 代表索引鍵，<c>{1}</c> 代表值
        /// DSL 參數: <c>dict-format</c>
        /// </summary>
        public const string DictFormat = "dict-format";

        /// <summary>
        /// 套用於字典索引鍵的格式字串
        /// DSL 參數: <c>key-format</c>
        /// </summary>
        public const string KeyFormat = "key-format";

        /// <summary>
        /// 套用於字典值的格式字串
        /// DSL 參數: <c>value-format</c>
        /// </summary>
        public const string ValueFormat = "value-format";
    }

    /// <summary>
    /// 提供轉換 (Convert)相關指令的 DSL 參數名稱常數
    /// </summary>
    internal static class CnvParams
    {
        /// <summary>
        /// 指定排序方式或排序參數
        /// DSL 參數: <c>sort</c>
        /// </summary>
        public const string Sort = "sort";

        /// <summary>
        /// 重量轉換參數
        /// DSL 參數: <c>weight</c>
        /// </summary>
        public const string Weight = "weight";

        public const string Length = "length";

        /// <summary>
        /// 溫度轉換參數
        /// DSL 參數: <c>temp</c>
        /// </summary>
        public const string Temp = "temp";
    }
}

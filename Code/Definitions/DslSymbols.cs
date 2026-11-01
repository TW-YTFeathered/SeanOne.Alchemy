// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Definitions;

/// <summary>
/// DSL 符號定義類別，集中管理 DSL 語法解析中使用的各種符號常數。
/// </summary>
internal class DslSymbols
{
    /// <summary>
    /// 引號符號，用於包裹字串值，標示其開始與結束。
    /// </summary>
    public const string QuoteSymbol = "\"";

    /// <summary>
    /// 參數前綴符號，用於標示 DSL 參數的起始。
    /// </summary>
    public const string ParamPrefix = "/";

    /// <summary>
    /// 參數分隔符號，用於分隔參數名稱與其對應的值。
    /// </summary>
    public const string ParamSeparator = ":";
}

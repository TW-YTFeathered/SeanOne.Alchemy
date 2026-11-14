// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Builder;

/// <summary>
/// Defines the parameter options used by <c>CnvFunctionBuilder</c> (cnv).
/// Handles unit conversion DSL parameters.
/// </summary>
public enum CnvParam
{
    /// <summary>
    /// Unit conversion for temperature (e.g. "C->F").
    /// DSL param: <c>temp</c>.
    /// </summary>
    Temp,

    /// <summary>
    /// Unit conversion for length (e.g. "M->KM").
    /// DSL param: <c>length</c>.
    /// </summary>
    Length,

    /// <summary>
    /// Unit conversion for weight (e.g. "Kg->G").
    /// DSL param: <c>weight</c>.
    /// </summary>
    Weight
}

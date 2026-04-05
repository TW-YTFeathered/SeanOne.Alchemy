// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;

namespace SeanOne.Alchemy.Builder
{
    /// <summary>
    /// Provides extension methods to encode strongly-typed parameters into DSL tokens.
    /// </summary>
    public static class AlchemyDslEncoder
    {
        /// <summary>
        /// Encodes temperature conversion units into a DSL string (e.g., "Celsius->Fahrenheit").
        /// </summary>
        public static string ToToken(this TemperatureUnit from, TemperatureUnit to)
            => $"{from}->{to}";

        /// <summary>
        /// Encodes sorting algorithms and direction into a DSL token (e.g., "asd" for ArraySort Descending).
        /// </summary>
        public static string ToToken(this SortAlgorithm algorithm, bool descending = false)
        {
            string token = algorithm.ToSortAlgorithmString();

            return descending ? $"{token}d" : token;
        }
    }
}

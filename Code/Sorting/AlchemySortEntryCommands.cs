// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections.Generic;

namespace SeanOne.Alchemy.Sorting
{
    /// <summary>
    /// 
    /// </summary>
    internal static class SortCommands
    {
        private static readonly StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;

        #region Command Aliases
        private static readonly HashSet<string> BubbleSortAliases = new HashSet<string>(comparer)
        {
            "bs", "bubble", "bubblesort"
        };

        private static readonly HashSet<string> BubbleSortDescendingAliases = new HashSet<string>(comparer)
        {
            "bsd", "bubbledescending", "bubblesortdescending"
        };

        private static readonly HashSet<string> LinqSortAliases = new HashSet<string>(comparer)
        {
            "ls", "linq", "linqsort"
        };

        private static readonly HashSet<string> LinqSortDescendingAliases = new HashSet<string>(comparer)
        {
            "lsd", "linqdescending", "linqsortdescending"
        };
        #endregion

        #region Command Validation Methods
        public static bool IsBubbleSortCommand(string command) => BubbleSortAliases.Contains(command);
        public static bool IsBubbleSortDescendingCommand(string command) => BubbleSortDescendingAliases.Contains(command);
        public static bool IsLinqSortCommand(string command) => LinqSortAliases.Contains(command);
        public static bool IsLinqSortDescendingCommand(string command) => LinqSortDescendingAliases.Contains(command);
        #endregion
    }
}

// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System.Collections;

namespace SeanOne.Alchemy.Sorting
{
    partial class ListSorter
    {
        public static void BubbleSortList(IList list, bool isDescending)
        {
            if (list == null || list.Count <= 1)
                return;

            bool swapped;
            int n = list.Count;

            do
            {
                swapped = false;
                for (int i = 1; i < n; i++)
                {
                    // 使用變數儲存比較結果，避免重複計算
                    int comparisonResult = s_Comparer.Compare(list[i - 1], list[i]);
                    bool shouldSwap = isDescending ?
                        comparisonResult < 0 :  // 降序: 前一個小於後一個時交換
                        comparisonResult > 0;   // 升序: 前一個大於後一個時交換

                    if (shouldSwap)
                    {
                        // 交換元素
                        list.Swap(i, ref swapped);
                    }
                }
                n--;
            } while (swapped);
        }
    }
}

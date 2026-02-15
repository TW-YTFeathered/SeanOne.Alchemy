// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Sorting
{
    partial class ArraySorter
    {
        private static void BubbleSortList<T>(T[] array, bool isDescending)
        {
            if (array == null || array.Length <= 1)
                return;

            bool swapped;
            int n = array.Length;

            do
            {
                swapped = false;
                for (int i = 1; i < n; i++)
                {
                    // 使用變數儲存比較結果，避免重複計算
                    int comparisonResult = s_Comparer.Compare(array[i - 1], array[i]);
                    bool shouldSwap = isDescending ?
                        comparisonResult < 0 :  // 降序: 前一個小於後一個時交換
                        comparisonResult > 0;   // 升序: 前一個大於後一個時交換

                    if (shouldSwap)
                    {
                        // 交換元素
                        array.Swap(i, ref swapped);
                    }
                }
                n--;
            } while (swapped);
        }
    }
}

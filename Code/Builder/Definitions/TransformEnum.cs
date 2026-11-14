// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Builder;

// 溫度轉換單位
public enum TempUnit { C, F, K }

// 長度轉換單位
public enum LengthUnit { A, NM, UM, MM, CM, M, KM, IN, FT, YD, MI, NMI }

// 重量轉換單位
public enum WeightUnit { Mg, Cg, Dg, G, Dag, Hg, Kg, T, Oz, Lb, St, ShortTon, LongTon }

// 排序演算法
public enum SortAlgorithm
{
    InsertionAsc, InsertionDesc,
    ArraySortAsc, ArraySortDesc,
    LinqAsc, LinqDesc
}

using System;
using System.Collections.Generic;

namespace FailoverClusters.WinForms;

internal static class ClusterHelp
{
	internal static byte[] Int64ToByteArray(long int64)
	{
		byte[] array = new byte[8];
		for (int i = 0; i < 8; i++)
		{
			array[array.Length - 1 - i] = (byte)(int64 & 0xFF);
			int64 >>= 8;
		}
		return array;
	}

	internal static List<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Comparison<TKey> comparer)
	{
		List<TSource> list = new List<TSource>(source);
		list.Sort((TSource x, TSource y) => comparer((x != null) ? keySelector(x) : default(TKey), (y != null) ? keySelector(y) : default(TKey)));
		return list;
	}
}


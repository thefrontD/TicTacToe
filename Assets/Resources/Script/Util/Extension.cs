using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// C# 체계에서 더 필요한 요소가 존재하는 경우 여기에 구현바람
/// </summary>
public static class Extensions
{
    private static Random rand = new Random();
    
    public static void Shuffle<T>(this IList<T> values)
    {
        for (int i = values.Count - 1; i > 0; i--) {
            int k = rand.Next(i + 1);
            T value = values[k];
            values[k] = values[i];
            values[i] = value;
        }
    }

    /// <summary>
    /// list에서 unique한 무작위 원소 count개를 뽑는다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="count">뽑는 개수</param>
    /// <returns></returns>
    public static List<T> ChooseDifferentRandomElements<T>(IEnumerable<T> list, int count)
    {
        List<T> pool = new List<T>(list);
        pool.Shuffle();
        return pool.Take(count).ToList();
    }

    /// <summary>
    /// min <= x < max를 만족하는 서로 다른 무작위 정수 x를 count개 뽑는다.
    /// </summary>
    /// <param name="minInclusive"></param>
    /// <param name="maxExclusive"></param>
    /// <param name="count">뽑는 개수</param>
    /// <returns></returns>
    public static List<int> ChooseDifferentRandomIntegers(int minInclusive, int maxExclusive, int count)
    {
        List<int> pool = Enumerable.Range(minInclusive, maxExclusive - minInclusive + 1).ToList();  // [ min, min+1, ..., max-1 ]
        pool.Shuffle();
        return pool.Take(count).ToList();
    }
}
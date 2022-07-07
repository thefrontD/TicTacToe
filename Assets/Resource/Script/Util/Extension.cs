using System;
using System.Collections.Generic;
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
}
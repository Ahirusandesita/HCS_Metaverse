using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectionExtends
{
    public static void CompleteDispose<T>(this ICollection<T> collections) where T : IDisposable
    {
        foreach(T t in collections)
        {
            t.Dispose();
        }

        collections.Clear();
    }
}
﻿using System.Collections.Generic;

namespace Game.Helpers
{
    public static class DictionaryExtensions
    {
        public static void AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary.Remove(key);
            dictionary.Add(key, value);
        }
    }
}
using System;
using System.Collections.Generic;

namespace Game_Elements.utility {
    public static class DictionaryEx {
        public static Dictionary<TKey, int> ConcatWithAddition<TKey>(
            this Dictionary<TKey, int> baseDictionary,
            Dictionary<TKey, int> dictionaryToConcat
        ) {
            var result = new Dictionary<TKey, int>(baseDictionary);
            foreach (var (key, value) in dictionaryToConcat) {
                if (result.ContainsKey(key)) {
                    result[key] += value;
                } else {
                    result[key] = value;
                }
            }
            return result;
        }
        
        public static void AppendWithAddition<TKey>(
            this Dictionary<TKey, int> baseDictionary,
            Dictionary<TKey, int> dictionaryToAppend
        ) {
            foreach (var (key, value) in dictionaryToAppend) {
                if (baseDictionary.ContainsKey(key)) {
                    baseDictionary[key] += value;
                } else {
                    baseDictionary[key] = value;
                }
            }
        }
        
        public static Dictionary<TKey, float> ConcatWithAddition<TKey>(
            this Dictionary<TKey, float> baseDictionary,
            Dictionary<TKey, float> dictionaryToConcat
        ) {
            var result = new Dictionary<TKey, float>(baseDictionary);
            foreach (var (key, value) in dictionaryToConcat) {
                if (result.ContainsKey(key)) {
                    result[key] += value;
                } else {
                    result[key] = value;
                }
            }
            return result;
        }
        
        public static void AppendWithAddition<TKey>(
            this Dictionary<TKey, float> baseDictionary,
            Dictionary<TKey, float> dictionaryToAppend
        ) {
            foreach (var (key, value) in dictionaryToAppend) {
                if (baseDictionary.ContainsKey(key)) {
                    baseDictionary[key] += value;
                } else {
                    baseDictionary[key] = value;
                }
            }
        }
    }
}
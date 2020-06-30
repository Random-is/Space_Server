using System;
using System.Collections.Generic;

namespace Game_Elements.utility {
    public static class DictionaryEx {
        public static Dictionary<TKey, int> ConcatWithAddition<TKey>(
            this Dictionary<TKey, int> baseDictionary,
            Dictionary<TKey, int> dictionaryToConcat
        ) {
            var result = new Dictionary<TKey, int>(baseDictionary);
            foreach (var element in dictionaryToConcat) {
                if (result.ContainsKey(element.Key)) {
                    result[element.Key] += element.Value;
                } else {
                    result[element.Key] = element.Value;
                }
            }
            return result;
        }
        
        public static void AppendWithAddition<TKey>(
            this Dictionary<TKey, int> baseDictionary,
            Dictionary<TKey, int> dictionaryToAppend
        ) {
            foreach (var element in dictionaryToAppend) {
                if (baseDictionary.ContainsKey(element.Key)) {
                    baseDictionary[element.Key] += element.Value;
                } else {
                    baseDictionary[element.Key] = element.Value;
                }
            }
        }
        
        public static Dictionary<TKey, float> ConcatWithAddition<TKey>(
            this Dictionary<TKey, float> baseDictionary,
            Dictionary<TKey, float> dictionaryToConcat
        ) {
            var result = new Dictionary<TKey, float>(baseDictionary);
            foreach (var element in dictionaryToConcat) {
                if (result.ContainsKey(element.Key)) {
                    result[element.Key] += element.Value;
                } else {
                    result[element.Key] = element.Value;
                }
            }
            return result;
        }
        
        public static void AppendWithAddition<TKey>(
            this Dictionary<TKey, float> baseDictionary,
            Dictionary<TKey, float> dictionaryToAppend
        ) {
            foreach (var element in dictionaryToAppend) {
                if (baseDictionary.ContainsKey(element.Key)) {
                    baseDictionary[element.Key] += element.Value;
                } else {
                    baseDictionary[element.Key] = element.Value;
                }
            }
        }
        
        public static void AddNotExistEnumKeysToDictionary<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary
        ) where TKey : Enum {
            foreach (TKey key in Enum.GetValues(typeof(TKey))) {
                if (!dictionary.ContainsKey(key)) {
                    dictionary[key] = default;
                }
            }
        }
    }
}
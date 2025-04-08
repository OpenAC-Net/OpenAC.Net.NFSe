// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

/// <summary>
/// Extensões para o Dicionario
/// </summary>
public static class DictionaryExtensions
{
    public static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) =>
        dictionary.GetValueOrDefault(key, default!);

    public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
        if (dictionary is null)
            throw new ArgumentNullException(nameof(dictionary));

        return dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;
    }
    
    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary is null)
            throw new ArgumentNullException(nameof(dictionary));

        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
            return true;
        }

        return false;
    }
}
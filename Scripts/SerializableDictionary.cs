using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��ųʸ��� Unity �ν����Ϳ��� �� �� �ֵ��� ��ȯ�ϴ� Ŭ����
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
[Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    public void Initialize()
    {
        dictionary.Clear();
        for (int i = 0; i < keys.Count; i++)
        {
            dictionary[keys[i]] = values[i];
        }
    }

    public Dictionary<TKey, TValue> GetDictionary()
    {
        return dictionary;
    }

    public void SetDictionary(Dictionary<TKey, TValue> source)
    {
        keys.Clear();
        values.Clear();

        foreach (var pair in source)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
}

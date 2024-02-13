using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] List<TKey> _keys;
    [SerializeField] List<TValue> _values;

    public void OnAfterDeserialize()
    {
        
    }

    public void OnBeforeSerialize()
    {
        
    }
}

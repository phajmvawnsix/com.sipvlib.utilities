using System.Collections.Generic;
using UnityEngine;

namespace SiPVLib.Utilities.Serialization
{
    /// <summary>
    /// Opts a <see cref="SerializableDictionary{TKey,TValue}"/> field into the paired key/value
    /// inspector drawer. Applied to the field rather than inferred from the type, because Unity
    /// resolves drawers for a serialized field through its attributes or its concrete type, and the
    /// drawer needs to reach the dictionary's private backing lists.
    /// </summary>
    public class SerializableDictionaryAttribute : PropertyAttribute
    {
    }

    /// <summary>
    /// A <see cref="Dictionary{TKey,TValue}"/> Unity can serialize, backed by parallel key/value
    /// lists. Deriving from <see cref="Dictionary{TKey,TValue}"/> (rather than wrapping one) keeps
    /// every call site — indexers, <c>TryGetValue</c>, iteration, and members typed as
    /// <c>Dictionary&lt;,&gt;</c> — working unchanged.
    ///
    /// Unity only serializes concrete, non-generic types, so a field must use a subclass that closes
    /// the generic arguments (see <see cref="SiPVLib.Sound.Configs.ConfigSoundGroup"/> for an example).
    ///
    /// This exists because SiPVLib no longer requires Odin Inspector, whose serializer was previously
    /// what persisted these dictionaries.
    /// </summary>
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private List<TKey> _keys = new();
        [SerializeField, HideInInspector] private List<TValue> _values = new();

        public SerializableDictionary()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> source) : base(source)
        {
        }

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var pair in this)
            {
                _keys.Add(pair.Key);
                _values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            var count = Mathf.Min(_keys.Count, _values.Count);
            for (var i = 0; i < count; i++)
            {
                var key = _keys[i];

                // A null or duplicate key is reachable purely by editing in the inspector (a freshly
                // added row starts blank), so drop it rather than throwing during deserialization.
                if (key == null || ContainsKey(key)) continue;

                Add(key, _values[i]);
            }
        }
    }
}

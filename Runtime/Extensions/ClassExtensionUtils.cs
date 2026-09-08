using UnityEngine;

namespace SiPVLib.Utilities.Extensions
{
    public static class ClassExtensionUtils
    {
        /// <summary>
        /// Deep-clones <paramref name="source"/> via <see cref="JsonUtility"/>. Covers
        /// Unity-serializable types only: dictionaries not backed by
        /// <see cref="SiPVLib.Utilities.Serialization.SerializableDictionary{TKey,TValue}"/>,
        /// interface-typed fields, and polymorphic fields without <c>[SerializeReference]</c> will
        /// not survive the round-trip.
        /// </summary>
        public static T DeepClone<T>(this T source) where T : class
        {
            if (source == null) return default;

            // ScriptableObject/MonoBehaviour need Instantiate: JsonUtility.FromJson can't construct
            // a UnityEngine.Object, and overwriting the original via FromJsonOverwrite would defeat
            // the point of cloning.
            if (source is ScriptableObject scriptableObject)
            {
                return Object.Instantiate(scriptableObject) as T;
            }

            return JsonUtility.FromJson<T>(JsonUtility.ToJson(source));
        }
    }
}

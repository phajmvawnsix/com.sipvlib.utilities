using UnityEngine;

namespace SiPVLib.Utilities.Extensions
{
    public static class GameObjectExtensionUtils
    {
        public static T GetOrAddComponent<T>(this GameObject source) where T : Component
        {
            return source.TryGetComponent<T>(out var component) ? component : source.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component source) where T : Component
        {
            return source.TryGetComponent<T>(out var component) ? component : source.gameObject.AddComponent<T>();
        }
    }
}

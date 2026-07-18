using System;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
#endif

namespace SiPVLib.Utilities.Extensions
{
    public static class ClassExtensionUtils
    {
        /// <summary>
        /// Deep-clones via Odin's serializer. Requires Odin Inspector to be installed (not bundled
        /// with this package); throws <see cref="NotSupportedException"/> otherwise.
        /// </summary>
        public static T DeepClone<T>(this T source) where T : class
        {
            if (source == null) return default;
#if ODIN_INSPECTOR
            var bytes = SerializationUtility.SerializeValue(source, DataFormat.Binary, out var referencedUnityObjects);
            return SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary, referencedUnityObjects);
#else
            throw new NotSupportedException(
                $"{nameof(DeepClone)} requires Odin Inspector to be installed (defines ODIN_INSPECTOR). " +
                "Odin is a paid Unity Asset Store asset and is not bundled with com.sipvlib.utilities.");
#endif
        }
    }
}

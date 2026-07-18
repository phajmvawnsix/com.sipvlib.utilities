using System;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
#endif

namespace SiPVLib.Utilities
{
    public static class OtherUtils
    {
        /// <summary>
        /// Computes an exponential backoff delay (in milliseconds) for the given retry attempt, optionally capped.
        /// </summary>
        /// <param name="retryCount">Number of retries already attempted.</param>
        /// <param name="maxRetryTime">Upper bound in milliseconds; pass 0 or less for no cap.</param>
        /// <returns>Delay in milliseconds before the next retry.</returns>
        public static int GetRetryTime(int retryCount, int maxRetryTime = 60000)
        {
            if (maxRetryTime <= 0)
            {
                return (int) (Math.Pow(2, retryCount) * 1000);
            }

            return (int)(Math.Clamp(Math.Pow(2, retryCount) * 1000, 0, maxRetryTime));
        }

        /// <summary>
        /// Serializes this object (including private/protected [SerializeField] members) via Odin's
        /// serializer and encodes the result as a Base64 string. Requires Odin Inspector to be installed
        /// (not bundled with this package); throws <see cref="NotSupportedException"/> otherwise.
        /// </summary>
        /// <returns>Base64-encoded binary representation of the object.</returns>
        public static string ToBase64String<T>(this T obj) where T : class
        {
#if ODIN_INSPECTOR
            var bytes = SerializationUtility.SerializeValue(obj, DataFormat.Binary);
            return Convert.ToBase64String(bytes);
#else
            throw new NotSupportedException(
                $"{nameof(ToBase64String)} requires Odin Inspector to be installed (defines ODIN_INSPECTOR). " +
                "Odin is a paid Unity Asset Store asset and is not bundled with com.sipvlib.utilities.");
#endif
        }
    }
}
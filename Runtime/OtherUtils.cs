using System;
using System.Text;
using UnityEngine;

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
        /// Serializes this object (including private/protected [SerializeField] members) via
        /// <see cref="JsonUtility"/> and encodes the result as a Base64 string.
        /// </summary>
        /// <returns>Base64-encoded representation of the object.</returns>
        public static string ToBase64String<T>(this T obj) where T : class
        {
            if (obj == null) return string.Empty;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonUtility.ToJson(obj)));
        }
    }
}

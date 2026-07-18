using System;
using System.Globalization;

namespace SiPVLib.Utilities.Extensions
{
    public static class LongExtensionUtils
    {
        public const long MinUnixSeconds = 0; // January 1, 1970
        public const long MaxUnixSeconds = 253402300799; // December 31, 9999

        // ── Time Validation ──────────────────────────────────────────────

        public static bool IsValidUnixSeconds(this long seconds)
        {
            return seconds is >= MinUnixSeconds and <= MaxUnixSeconds;
        }

        public static bool IsValidUnixMilliseconds(this long milliseconds)
        {
            return milliseconds is >= MinUnixSeconds * 1000 and <= MaxUnixSeconds * 1000;
        }

        // ── Time Conversions ─────────────────────────────────────────────

        public static long ToUnixSeconds(this DateTime dateTime)
        {
            return DateTimeOffset.FromFileTime(dateTime.ToFileTimeUtc()).ToUnixTimeSeconds();
        }

        public static long ToUnixMilliseconds(this DateTime dateTime)
        {
            return DateTimeOffset.FromFileTime(dateTime.ToFileTimeUtc()).ToUnixTimeMilliseconds();
        }

        public static DateTime FromUnixSeconds(this long seconds, bool safe = true)
        {
            seconds = ResolveUnixSeconds(seconds, safe);
            return DateTimeOffset.FromUnixTimeSeconds(seconds).DateTime;
        }

        public static DateTime FromUnixSecondsUtc(this long seconds, bool safe = true)
        {
            seconds = ResolveUnixSeconds(seconds, safe);
            return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
        }

        public static DateTime FromUnixMilliseconds(this long milliseconds, bool safe = true)
        {
            milliseconds = ResolveUnixMilliseconds(milliseconds, safe);
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).DateTime;
        }

        public static DateTime FromUnixMillisecondsUtc(this long milliseconds, bool safe = true)
        {
            milliseconds = ResolveUnixMilliseconds(milliseconds, safe);
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime;
        }

        // ── Time Comparison ──────────────────────────────────────────────

        public static bool IsSameDay(this DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1.Date == dateTime2.Date;
        }

        public static bool IsSameDayUtc(this DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1.ToUniversalTime().Date == dateTime2.ToUniversalTime().Date;
        }

        public static bool IsSameDay(this long unixSeconds1, long unixSeconds2)
        {
            var dateTime1 = unixSeconds1.FromUnixSeconds();
            var dateTime2 = unixSeconds2.FromUnixSeconds();
            return dateTime1.IsSameDay(dateTime2);
        }

        public static bool IsSameDayUtc(this long unixSeconds1, long unixSeconds2)
        {
            var dateTime1 = unixSeconds1.FromUnixSecondsUtc();
            var dateTime2 = unixSeconds2.FromUnixSecondsUtc();
            return dateTime1.IsSameDayUtc(dateTime2);
        }

        public static bool IsSameWeek(this DateTime dateTime1, DateTime dateTime2)
        {
            var calendar = CultureInfo.CurrentCulture.Calendar;
            var week1 = calendar.GetWeekOfYear(dateTime1, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            var week2 = calendar.GetWeekOfYear(dateTime2, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return dateTime1.Year == dateTime2.Year && week1 == week2;
        }

        public static bool IsSameWeekUtc(this DateTime dateTime1, DateTime dateTime2)
        {
            var calendar = CultureInfo.InvariantCulture.Calendar;
            var week1 = calendar.GetWeekOfYear(dateTime1.ToUniversalTime(),
                CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            var week2 = calendar.GetWeekOfYear(dateTime2.ToUniversalTime(),
                CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return dateTime1.ToUniversalTime().Year == dateTime2.ToUniversalTime().Year && week1 == week2;
        }

        public static bool IsSameWeek(this long unixSeconds1, long unixSeconds2)
        {
            var dateTime1 = unixSeconds1.FromUnixSeconds();
            var dateTime2 = unixSeconds2.FromUnixSeconds();
            return dateTime1.IsSameWeek(dateTime2);
        }

        public static bool IsSameMonth(this DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1.Year == dateTime2.Year && dateTime1.Month == dateTime2.Month;
        }

        public static bool IsSameMonthUtc(this DateTime dateTime1, DateTime dateTime2)
        {
            var utc1 = dateTime1.ToUniversalTime();
            var utc2 = dateTime2.ToUniversalTime();
            return utc1.Year == utc2.Year && utc1.Month == utc2.Month;
        }

        public static bool IsSameMonth(this long unixSeconds1, long unixSeconds2)
        {
            var dateTime1 = unixSeconds1.FromUnixSeconds();
            var dateTime2 = unixSeconds2.FromUnixSeconds();
            return dateTime1.IsSameMonth(dateTime2);
        }

        public static bool IsSameYear(this DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1.Year == dateTime2.Year;
        }

        public static bool IsSameYearUtc(this DateTime dateTime1, DateTime dateTime2)
        {
            var utc1 = dateTime1.ToUniversalTime();
            var utc2 = dateTime2.ToUniversalTime();
            return utc1.Year == utc2.Year;
        }

        public static bool IsSameYear(this long unixSeconds1, long unixSeconds2)
        {
            var dateTime1 = unixSeconds1.FromUnixSeconds();
            var dateTime2 = unixSeconds2.FromUnixSeconds();
            return dateTime1.IsSameYear(dateTime2);
        }

        // ── Private Helpers ──────────────────────────────────────────────

        private static long ResolveUnixSeconds(long seconds, bool safe)
        {
            if (seconds.IsValidUnixSeconds()) return seconds;

            if (!safe)
            {
                throw new ArgumentOutOfRangeException(nameof(seconds),
                    $"Value must be between {MinUnixSeconds} and {MaxUnixSeconds}.");
            }

            return Math.Clamp(seconds, MinUnixSeconds, MaxUnixSeconds);
        }

        private static long ResolveUnixMilliseconds(long milliseconds, bool safe)
        {
            if (milliseconds.IsValidUnixMilliseconds()) return milliseconds;

            if (!safe)
            {
                throw new ArgumentOutOfRangeException(nameof(milliseconds),
                    $"Value must be between {MinUnixSeconds * 1000} and {MaxUnixSeconds * 1000}.");
            }

            return Math.Clamp(milliseconds, MinUnixSeconds * 1000, MaxUnixSeconds * 1000);
        }
    }
}

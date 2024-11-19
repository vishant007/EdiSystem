using System;

namespace EDI315Parser.Helpers
{
    public static class DateTimeHelper
    {
        public static DateOnly ParseDateOnly(string dateStr)
        {
            if (DateTime.TryParseExact(dateStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dateTime))
            {
                return DateOnly.FromDateTime(dateTime);  // Convert DateTime to DateOnly
            }
            else
            {
                return default;  // Return default DateOnly (0001-01-01) if parsing fails
            }
        }
    }
}

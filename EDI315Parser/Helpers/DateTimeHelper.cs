using System;

namespace EDI315Parser.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime? ParseDateTime(string dateTimeStr)
        {
            return DateTime.TryParse(dateTimeStr, out DateTime result) ? result : null;
        }
    }
}

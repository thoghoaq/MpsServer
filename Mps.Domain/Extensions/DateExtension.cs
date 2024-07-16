using Mps.Domain.Enums;
using System.Globalization;

namespace Mps.Domain.Extensions
{
    public static class DateExtension
    {
        public static bool CompareMonth(this DateTime date, DateTime compareDate)
        {
            return date.Year == compareDate.Year && date.Month == compareDate.Month;
        }

        public static bool CompareYear(this DateTime date, DateTime compareDate)
        {
            return date.Year == compareDate.Year;
        }

        public static bool CompareWeek(this DateTime date, DateTime compareDate)
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            var calendar = cultureInfo.Calendar;

            int week1 = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            int week2 = calendar.GetWeekOfYear(compareDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

            int year1 = calendar.GetYear(date);
            int year2 = calendar.GetYear(compareDate);

            return year1 == year2 && week1 == week2;
        }

        public static bool InPayoutDate(this DateTime date, DateTime monthToDate, PayoutDate payoutDate)
        {
            switch (payoutDate)
            {
                case PayoutDate.Day1:
                    return date.InRange(monthToDate.StartOfDay().LastMonth().Day(22), monthToDate.EndOfDay().EndOfLastMonth());
                case PayoutDate.Day8:
                    return date.InRange(monthToDate.StartOfDay().StartOfMonth(), monthToDate.EndOfDay().Day(7));
                case PayoutDate.Day15:
                    return date.InRange(monthToDate.StartOfDay().Day(8), monthToDate.EndOfDay().Day(14));
                case PayoutDate.Day22:
                    return date.InRange(monthToDate.StartOfDay().Day(15), monthToDate.EndOfDay().Day(21));
                default:
                    return false;
            }
        }

        public static DateTime EndOfMonth(this DateTime date)
        {
            return date.AddDays(-date.Day + 1).AddMonths(1).AddDays(-1);
        }

        public static DateTime StartOfMonth(this DateTime date)
        {
            return date.AddDays(-date.Day + 1);
        }

        public static DateTime Day(this DateTime date, int day)
        {
            return date.AddDays(-date.Day + day);
        }

        public static DateTime LastMonth(this DateTime date)
        {
            return date.AddMonths(-1);
        }

        public static DateTime EndOfLastMonth(this DateTime date)
        {
            return date.LastMonth().EndOfMonth();
        }

        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, kind: DateTimeKind.Utc);
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, kind: DateTimeKind.Utc);
        }

        public static bool InRange(this DateTime date, DateTime startDate, DateTime endDate)
        {
            return date >= startDate && date <= endDate;
        }
    }
}

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
            var cultureInfo = CultureInfo.CurrentCulture;
            var calendar = cultureInfo.Calendar;

            int week1 = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            int week2 = calendar.GetWeekOfYear(compareDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

            int year1 = calendar.GetYear(date);
            int year2 = calendar.GetYear(compareDate);

            return year1 == year2 && week1 == week2;
        }
    }
}

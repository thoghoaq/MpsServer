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
    }
}

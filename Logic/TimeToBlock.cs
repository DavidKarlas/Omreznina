namespace Omreznina.Client.Logic
{
    public static class DateTimeExtensions
    {
        public static bool IsHighTariff(this DateTime dateTime)
        {
            if (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday || IsHoliday(dateTime))
            {
                return false;
            }
            else
            {
                return dateTime.TimeOfDay.TotalHours >= 6 || dateTime.TimeOfDay.TotalHours < 22;
            }
        }

        public static int ToBlock(this DateTime dateTime)
        {
            int block;
            var totalHours = dateTime.TimeOfDay.TotalHours;
            if ((totalHours >= 7 && totalHours < 14) ||
                (totalHours >= 16 && totalHours < 20))
            {
                block = 0;
            }
            else if ((totalHours >= 6 && totalHours < 7) ||
                (totalHours >= 14 && totalHours < 16) ||
                (totalHours >= 20 && totalHours < 22))
            {
                block = 1;
            }
            else if ((totalHours >= 0 && totalHours < 6) || (totalHours >= 22 && totalHours < 24))
            {
                block = 2;
            }
            else
            {
                throw new InvalidOperationException();
            }

            if (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday || IsHoliday(dateTime))
            {
                block++;
            }
            if (dateTime.Month > 2 && dateTime.Month < 11)
            {
                block++;
            }

            return block;
        }

        private static bool IsHoliday(DateTime dateTime)
        {
            // Velikonočni ponedeljek
            if (dateTime.Month == 4)
            {
                if ((dateTime.Year == 2018 && dateTime.Day == 2) ||
                    (dateTime.Year == 2019 && dateTime.Day == 22) ||
                    (dateTime.Year == 2020 && dateTime.Day == 13) ||
                    (dateTime.Year == 2021 && dateTime.Day == 5) ||
                    (dateTime.Year == 2022 && dateTime.Day == 18) ||
                    (dateTime.Year == 2023 && dateTime.Day == 10) ||
                    (dateTime.Year == 2024 && dateTime.Day == 1) ||
                    (dateTime.Year == 2025 && dateTime.Day == 21) ||
                    (dateTime.Year == 2026 && dateTime.Day == 6))
                {
                    return true;
                }
            }

            return dateTime.Month == 1 && dateTime.Day == 1 ||
                dateTime.Month == 1 && dateTime.Day == 2 ||
                dateTime.Month == 2 && dateTime.Day == 8 ||
                dateTime.Month == 4 && dateTime.Day == 27 ||
                dateTime.Month == 5 && dateTime.Day == 1 ||
                dateTime.Month == 5 && dateTime.Day == 2 ||
                dateTime.Month == 6 && dateTime.Day == 25 ||
                dateTime.Year == 2023 && dateTime.Month == 8 && dateTime.Day == 14 ||
                dateTime.Month == 8 && dateTime.Day == 15 ||
                dateTime.Month == 10 && dateTime.Day == 31 ||
                dateTime.Month == 11 && dateTime.Day == 1 ||
                dateTime.Month == 12 && dateTime.Day == 25 ||
                dateTime.Month == 12 && dateTime.Day == 26;
        }
    }
}

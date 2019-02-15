using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantBook.Models.Utilities
{
    public static class QuantHelper
    {

        public static DateTime get_first_day_of_month(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime get_last_day_of_month(DateTime date)
        {
            DateTime firstday = new DateTime(date.Year, date.Month, 1);
            return firstday.AddMonths(1).AddDays(-1);
        }

        


        public static DateTime get_workday(DateTime dt)
        {

            DateTime dt1 = dt;
            if (dt.DayOfWeek.ToString() == "Saturday")
                dt1 = dt - TimeSpan.FromDays(1);
            if (dt.DayOfWeek.ToString() == "Sunday")
                dt1 = dt - TimeSpan.FromDays(2);

            return dt1;
        }

        public static DateTime get_next_workday(DateTime date)
        {
            do
            {
                date = date.AddDays(1);
            }
            while (date.DayOfWeek == DayOfWeek.Saturday ||
                   date.DayOfWeek == DayOfWeek.Sunday ||
                   IsHoliday(date));

            return date;
        }


        public static DateTime get_previous_workday(DateTime date)
        {
            do
            {
                date = date.AddDays(-1);
            }
            while (date.DayOfWeek == DayOfWeek.Saturday ||
                   date.DayOfWeek == DayOfWeek.Sunday ||
                   IsHoliday(date));

            return date;
        }

        public static int get_number_calendar_days(DateTime startDate, DateTime endDate)
        {
            DateTime date = startDate;
            int num = 0;
            while (date <= endDate)
            {
                date = date.AddDays(1);
                num++;
            }
            return num - 1;
        }



        public static int get_number_workdays(DateTime startDate, DateTime endDate)
        {
            DateTime date = startDate;
            int num = 0;
            while (date <= endDate)
            {
                date = get_next_workday(date);
                num++;
            }
            return num;
        }

        private static bool IsHoliday(DateTime date)
        {
            string ss = @"SELECT * FROM Holiday WHERE HDate = '" + date + "'";
            //DataTable holidayTable = xu_data.sql_load(ss);
            bool res = false;
            //if (holidayTable.Rows.Count > 0)
            //    res = true;
            return res;
        }
    }
}

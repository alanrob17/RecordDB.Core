using Heinemann.Components;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecordDB.DAL.Extensions
{
    public static class DateTimeExtensions
    {
        // For object type (handles DBNull, null, etc.)
        public static string ToShortDate(this object value)
        {
            if (value == null || value == DBNull.Value)
                return "unk";

            try
            {
                DateTime dt = Convert.ToDateTime(value);
                return Dates.ShortDateString(dt);
            }
            catch
            {
                return "unk";
            }
        }

        // For nullable DateTime
        public static string ToShortDate(this DateTime? date)
        {
            if (!date.HasValue)
                return "unk";

            return Dates.ShortDateString(date.Value);
        }

        // For DateTime
        public static string ToShortDate(this DateTime date)
        {
            return Dates.ShortDateString(date);
        }

        // For string (if dates come as strings)
        public static string ToShortDate(this string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return "unk";

            if (DateTime.TryParse(dateString, out DateTime date))
                return Dates.ShortDateString(date);

            return "unk";
        }
    }
}

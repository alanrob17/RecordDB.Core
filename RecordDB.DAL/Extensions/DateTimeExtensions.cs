using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecordDB.DAL.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToShortDate(object? bought)
        {
            if (bought is DateTime dt)
                return dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);

            return "unk";
        }
    }
}

using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Emeint.Core.BE.Utilities
{
    public static class DateTimeUtility
    {
        private const string DATE_TIME_COMPACT_FORMAT = "yyyyMMddHHmmss";
        private const string DATE_TIME_12Hour_DISPLAY_FORMAT = "dd/MM/yyyy hh:mm tt";
        private const string DATE_TIME_HIGH_PRECISION_FORMAT = "yyyy-MM-dd'T'HH:mm:ss.ffffff";

        /// <summary>
        /// Convert from compact string date format to date display format
        /// convert from (yyyyMMddHHmmss) format to (dd/MM/yyyy HH:mm:ss)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static String FormatToDisplayDate(string date)
        {
            if (string.IsNullOrEmpty(date))
                throw new MissingParameterException("date", date);

            DateTime dateTime = new DateTime();


            if (!DateTime.TryParseExact(date, DATE_TIME_COMPACT_FORMAT, null, DateTimeStyles.None, out dateTime))
                throw new InvalidParameterException("date", date);

            return ConvertDateToString(dateTime, DateStringFormat.DateTime12HourDisplayFormat);
        }

        /// <summary>
        /// convert from date to string with defined format
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static String ConvertDateToString(DateTime date, DateStringFormat format)
        {
            string stringFormat = null;
            if (format == DateStringFormat.DateTimeCompactFormat)
                stringFormat = DATE_TIME_COMPACT_FORMAT;
            else if (format == DateStringFormat.DateTime12HourDisplayFormat)
                stringFormat = DATE_TIME_12Hour_DISPLAY_FORMAT;
            else if (format == DateStringFormat.DateTimeHighPrecisionFormat)
                stringFormat = DATE_TIME_HIGH_PRECISION_FORMAT;

            return date.ToString(stringFormat);
        }

        /// <summary>
        /// Convert from string to date 
        /// </summary>
        /// <param name="dateTimeString"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime FromDateTimeString(string dateTimeString, DateStringFormat format = DateStringFormat.DateTimeCompactFormat)
        {

            string stringFormat = null;
            if (format == DateStringFormat.DateTimeCompactFormat)
                stringFormat = DATE_TIME_COMPACT_FORMAT;
            else if (format == DateStringFormat.DateTime12HourDisplayFormat)
                stringFormat = DATE_TIME_12Hour_DISPLAY_FORMAT;
            else if (format == DateStringFormat.DateTimeHighPrecisionFormat)
                stringFormat = DATE_TIME_HIGH_PRECISION_FORMAT;
            DateTime.TryParseExact(dateTimeString, stringFormat, null, DateTimeStyles.None, out DateTime dateTime);
            return dateTime;
        }


        public static DateTimezone ConvertUtcDateTimeToUserDateTime(DateTime dateUtc, string timezone, decimal utcOffset)
        {
            string offsetSign = (utcOffset < 0) ? "+" : "-";
            return new DateTimezone
            {
                DateTime = dateUtc.AddMinutes((double)-utcOffset),
                Format = timezone
                //Format = $"GMT{offsetSign}{TimeSpan.FromMinutes((double)utcOffset).ToString(@"hh\:mm")} - {timezone}"
            };
        }

        public static int ConvertUtcHourToUserHour(int hour, decimal utcOffset)
        {
            TimeSpan utcDifferentOffset = TimeSpan.FromMinutes((double)utcOffset);

            // Get the integer value of the UTC offset
            int offsetInMinutes = (int)utcDifferentOffset.TotalMinutes;

            // Convert the minutes to hours and get the integer value
            int offsetInHours = offsetInMinutes / 60;

            return (hour - (offsetInHours)) % 24;

        }

        public static DateTime ConvertUserDateTimeToUtcDateTime(DateTime dateUtc, decimal utcOffset)
        {
            return dateUtc.AddMinutes((double)+utcOffset);      
        }
    }

}

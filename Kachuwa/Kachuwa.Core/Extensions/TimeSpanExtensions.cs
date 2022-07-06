using System;

namespace Kachuwa.Extensions
{
    public static class TimeSpanExtensions
    {
        public static long ToTotalSeconds(this string timeFormat)
        {
            var x = TimeSpan.Parse(timeFormat);
            return (long)x.TotalSeconds;
        }


        public static string ToHourMinute(this int totalSeconds)
        {
            string format1 = "{0} hr {1} min";
            string format2 = "{0} min";
            int hour = 0;
            int min = 0;

            if (totalSeconds <= 60)
            {
                min = 1;
                return string.Format(format2, min);
            }
            else
            {
              
                min = totalSeconds / 60;
                if (min >= 60)
                {
                    hour = min / 60;
                    min = min % 60;
                    return string.Format(format1, hour, min);
                }
                else
                {
                    return string.Format(format2, min);

                }
            }

        }
        public static string ToHourMinute(this string timeFormat)
        {
            int totalSeconds = TimeSpan.Parse(timeFormat).Seconds;
            string format1 = "{0} hr {1} min";
            string format2 = "{0} min";
            int hour = 0;
            int min = 0;

            if (totalSeconds <= 60)
            {
                min = 1;
                return string.Format(format2, min);
            }
            else
            {

                min = totalSeconds / 60;
                if (min >= 60)
                {
                    hour = min / 60;
                    min = min % 60;
                    return string.Format(format1, hour, min);
                }
                else
                {
                    return string.Format(format2, min);

                }
            }

        }
    }
}
﻿using System;
using Kachuwa.Web.Middleware;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Web.Extensions
{
    public static  class DateTimeExtensions
    {
        public static string ToAboutAgo(this DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("about {0} {1} ago",
                years, years == 1 ? "year" : "years");
            }
            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format("about {0} {1} ago",
                months, months == 1 ? "month" : "months");
            }
            if (span.Days > 0)
                return String.Format("about {0} {1} ago",
                span.Days, span.Days == 1 ? "day" : "days");
            if (span.Hours > 0)
                return String.Format("about {0} {1} ago",
                span.Hours, span.Hours == 1 ? "hour" : "hours");
            if (span.Minutes > 0)
                return String.Format("about {0} {1} ago",
                span.Minutes, span.Minutes == 1 ? "minute" : "minutes");
            if (span.Seconds > 5)
                return String.Format("about {0} seconds ago", span.Seconds);
            if (span.Seconds <= 5)
                return "just now";
            return string.Empty;
        }
        public static DateTime AsSysLocalToUtc(this DateTime dt,HttpContext context =null)
        {
            var _contenxt = context;
            if (_contenxt == null)
            {
                _contenxt = ContextResolver.Context;
            }
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(_contenxt.Items[HeaderConstants.TimeZoneStandardName].ToString());
            var utcDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dt, DateTimeKind.Unspecified), tz);
            return utcDate;
        }
        public static DateTime? AsSysLocalToUtc(this DateTime? dt, HttpContext context = null)
        {
            if (!dt.HasValue)
            {
                return dt;
            }
            var _contenxt = context;
            if (_contenxt == null)
            {
                _contenxt = ContextResolver.Context;
            }
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(_contenxt.Items[HeaderConstants.TimeZoneStandardName].ToString());
            var utcDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dt.Value, DateTimeKind.Unspecified), tz);
            return utcDate;
        }
    }
   
}

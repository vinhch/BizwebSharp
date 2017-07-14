﻿using System;

namespace BizwebSharp
{
    internal static class DateTimeExtensions
    {
        public static double ToUnixTimestamp(this DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentStackOverflow
{
    public static class IntegerExtensions
    {
        public static TimeSpan Days(this int value)
        {
            return TimeSpan.FromDays(value);
        }
    }
}

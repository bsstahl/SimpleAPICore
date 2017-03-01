using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using StackOverflow.Model;

namespace FluentStackOverflow
{
    public static class Asked
    {
        public static Func<Post, bool> InLast(TimeSpan span)
        {
            return p => p.CreationDate > DateTime.UtcNow.Subtract(span);
        }
    }
}

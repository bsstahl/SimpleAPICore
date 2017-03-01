using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackOverflow.Model;

namespace FluentStackOverflow
{
    public static class PostExtensions
    {
        public static IEnumerable<Post> Questions(this IEnumerable<Post> posts)
        {
            return posts.Where(p => p.ParentId == null);
        }

        public static IEnumerable<Post> WithAcceptedAnswer(this IEnumerable<Post> posts)
        {
            return posts.Where(p => p.AcceptedAnswerId != null);
        }

        public static IEnumerable<Post> TaggedWith(this IEnumerable<Post> posts, string tag)
        {
            return posts.Where(p => p.Tags.Contains($"<{tag.ToLowerInvariant()}>"));
        }
    }
}

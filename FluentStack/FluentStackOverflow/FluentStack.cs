using System;
using System.Collections.Generic;
using StackOverflow.Model;

namespace FluentStackOverflow
{
    public static class FluentStack
    {
        private const string _stackoverflowServiceRoot = "http://localhost:54290/StackOverflowData.svc/";


        public static IEnumerable<Post> Posts
        {
            get { return new StackOverflowService.Entities(new Uri(_stackoverflowServiceRoot)).Posts; }
        }

        public static IEnumerable<Post> Questions
        {
            get { return FluentStack.Posts.Questions(); }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackOverflow.Model;

namespace StackOverflowService
{
    public class PostCollection : List<Post>
    {
        public void Add(int id, string title, int? parentId, int? acceptedAnswerId, DateTime creationDate, string body, string tags)
        {
            var post = new Post()
            {
                ID = id,
                Title = title,
                ParentId = parentId,
                AcceptedAnswerId = acceptedAnswerId,
                CreationDate = creationDate,
                Body = body,
                Tags = tags
            };

            this.Add(post);
        }

    }
}

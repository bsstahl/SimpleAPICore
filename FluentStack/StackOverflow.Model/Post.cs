using System;
using System.Linq;

namespace StackOverflow.Model
{
    public class Post
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int? ParentId { get; set; }
        public int? AcceptedAnswerId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }
        public Post Parent { get; set; }
    }
}

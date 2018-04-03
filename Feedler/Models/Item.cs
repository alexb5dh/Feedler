using System;

namespace Feedler.Models
{
    public sealed class Item
    {
        public string Title { get; set; }

        public string Link { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public DateTime PublishedAt { get; set; }
    }
}
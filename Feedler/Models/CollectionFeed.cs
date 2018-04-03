namespace Feedler.Models
{
    public class CollectionFeed
    {
        public virtual Collection Collection { get; private set; }

        public virtual Feed Feed { get; private set; }

        protected CollectionFeed() { }

        public CollectionFeed(Collection collection, Feed feed)
        {
            Collection = collection;
            Feed = feed;
        }
    }
}
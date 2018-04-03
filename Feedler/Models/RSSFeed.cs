namespace Feedler.Models
{
    public sealed class RSSFeed: Feed
    {
        public string Uri { get; private set; }

        private RSSFeed() { }

        public RSSFeed(string id, string uri): base(id)
        {
            Uri = uri;
        }

        public override bool DeepEquals(Feed other)
        {
            return other?.GetType() == typeof(RSSFeed) && base.DeepEquals(other)
                && Uri.Equals(((RSSFeed)other).Uri);
        }
    }
}
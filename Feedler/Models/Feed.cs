using System;

namespace Feedler.Models
{
    public abstract class Feed: IEquatable<Feed>
    {
        public string Id { get; private set; }

        protected Feed() { }

        protected Feed(string id)
        {
            Id = id;
        }

        #region Equality members

        public bool Equals(Feed other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Feed)obj);
        }

        public override int GetHashCode() => Id?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0;

        public static bool operator ==(Feed left, Feed right) => Equals(left, right);

        public static bool operator !=(Feed left, Feed right) => !Equals(left, right);

        public virtual bool DeepEquals(Feed other) => string.Equals(Id, other?.Id, StringComparison.OrdinalIgnoreCase);

        #endregion

    }
}
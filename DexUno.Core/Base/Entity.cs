namespace Dex.Core.Base
{
    public class Entity<T>
    {
        public T Id { get; set; }

        public static bool operator !=(Entity<T> a, Entity<T> b) => !(a == b);

        public static bool operator ==(Entity<T> a, Entity<T> b)
        {
            if (a == null && b == null)
                return true;

            if (a == null || b == null)
                return false;

            return a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Entity<T>;

            if (other == null)
                return false;

            if (this == other)
                return true;

            if (GetType() != other.GetType())
                return false;

            return Id.Equals(other.Id);
        }

        public override int GetHashCode() => (GetType().ToString() + Id.ToString()).GetHashCode();
    }
}
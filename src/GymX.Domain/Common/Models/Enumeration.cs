using System.Reflection;

namespace GymX.Domain.Common.Models
{
    public abstract class Enumeration : IComparable
    {
        public string Name { get; }
        public int Id { get; }
        protected Enumeration(int id, string name) => (Id, Name) = (id, name);
        public override string ToString() => Name;
        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .OfType<T>();
        public override bool Equals(object? obj)
        {
            if (obj is not Enumeration otherValue)
                return false;
            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Id.Equals(otherValue.Id);
            return typeMatches && valueMatches;
        }
        public override int GetHashCode() => Id.GetHashCode();
        public int CompareTo(object? other) => Id.CompareTo(((Enumeration)other!).Id);
    }
}

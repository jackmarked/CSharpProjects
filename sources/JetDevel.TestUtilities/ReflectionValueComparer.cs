namespace JetDevel.TestUtilities;

public sealed partial class ReflectionValueComparer : IEqualityComparer<object>
{
    public new bool Equals(object? x, object? y)
    {
        var comparer = new InternalComparer();
        var result = comparer.Equals(x, y);
        return result;
    }

    public int GetHashCode(object obj)
    {
        if (obj == null)
            return 0;
        return obj.GetHashCode();
    }
}
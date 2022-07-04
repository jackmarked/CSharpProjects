using System.Runtime.CompilerServices;

namespace JetDevel.TestUtilities;

public sealed partial class ReflectionValueComparer
{
    // nested types...
    private sealed class ReferenceComparer: IEqualityComparer<object>
    {
        bool IEqualityComparer<object>.Equals(object? x, object? y) =>
            ReferenceEquals(x, y);

        int IEqualityComparer<object>.GetHashCode(object obj) =>
            RuntimeHelpers.GetHashCode(obj);
    }
}
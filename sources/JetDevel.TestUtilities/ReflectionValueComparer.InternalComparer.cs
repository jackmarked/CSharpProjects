using System.Reflection;

namespace JetDevel.TestUtilities;

public sealed partial class ReflectionValueComparer
{
    private sealed class InternalComparer
    {
        readonly Dictionary<object, int> referenceToNumberMapFirst;
        readonly Dictionary<object, int> referenceToNumberMapSecond;

        public InternalComparer()
        {
            referenceToNumberMapFirst = new(new ReferenceComparer());
            referenceToNumberMapSecond = new(new ReferenceComparer());
        }

        public new bool Equals(object? first, object? second)
        {
            if(first == null || second == null)
                return false;
            return EqualsInternal(first, second);
        }

        // private methods...
        private bool BothReferenceIsCachedOrNotCached(object first, object second)
        {
            return referenceToNumberMapFirst.ContainsKey(first) == referenceToNumberMapSecond.ContainsKey(second);
        }
        private bool BothReferenceCached(object first, object second)
        {
            return referenceToNumberMapFirst.ContainsKey(first) && referenceToNumberMapSecond.ContainsKey(second);
        }
        private bool BothReferenceHasSameIndex(object first, object second)
        {
            int firstIndex = referenceToNumberMapFirst[first];
            int secondIndex = referenceToNumberMapSecond[second];
            return firstIndex == secondIndex;
        }
        private void AddReferencesToCache(object first, object second)
        {
            referenceToNumberMapFirst.Add(first, referenceToNumberMapFirst.Count);
            referenceToNumberMapSecond.Add(second, referenceToNumberMapSecond.Count);
        }
        private bool EqualsInternal(object first, object second)
        {
            var firstType = first.GetType();
            var secondType = second.GetType();
            if(!firstType.Equals(secondType))
                return false;
            return EqualsTyped(first, second, firstType);
        }
        private bool EqualsTyped(object first, object second, Type type)
        {
            if(type.IsPrimitive || type.IsEnum || type == typeof(string))
                return first.Equals(second);

            if(type.IsClass)
            {
                if(BothReferenceIsCachedOrNotCached(first, second))
                {
                    if(BothReferenceCached(first, second))
                        return BothReferenceHasSameIndex(first, second);
                    else
                        AddReferencesToCache(first, second);
                }
                else
                    return false;
            }

            if(type.IsArray)
                return ArrayEquals((Array)first, (Array)second);
            if(type == typeof(Pointer))
                return PointerEquals((Pointer)first, (Pointer)second);
            return FieldsEquals(first, second, type);
        }
        private static unsafe bool PointerEquals(Pointer pointer1, Pointer pointer2)
        {
            IntPtr p1 = new(Pointer.Unbox(pointer1));
            IntPtr p2 = new(Pointer.Unbox(pointer2));
            return p1.Equals(p2);
        }
        private bool ArrayEquals(Array array1, Array array2)
        {
            var rank = array1.Rank;
            if(rank != array2.Rank)
                return false;
            for(int i = 0; i < rank; i++)
            {
                if(array1.GetLowerBound(i) !=
                  array2.GetLowerBound(i))
                    return false;
                if(array1.GetLongLength(i) !=
                  array2.GetLongLength(i))
                    return false;
            }
            var en1 = array1.GetEnumerator();
            var en2 = array2.GetEnumerator();

            bool moveNext1;
            bool moveNext2;
            do
            {
                moveNext1 = en1.MoveNext();
                moveNext2 = en2.MoveNext();
                if(moveNext1 ^ moveNext2)
                    return false;
                if(!moveNext1)
                    break;
                var current1 = en1.Current;
                var current2 = en2.Current;
                var current1IsNotNull = current1 != null;
                var current2IsNotNull = current2 != null;
                if(current1IsNotNull ^ current2IsNotNull)
                    return false;
                if(current1IsNotNull && !EqualsInternal(en1.Current, en2.Current))
                    return false;
            } while(moveNext1);

            return true;
        }
        private bool FieldsEquals(object first, object second, Type type)
        {
            var localType = type;
            while(localType != null)
            {
                var fields = localType.GetFields(BindingFlags.Instance
                  | BindingFlags.Public
                  | BindingFlags.NonPublic
                  | BindingFlags.DeclaredOnly);
                for(int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];

                    var firstValue = field.GetValue(first);
                    var secondValue = field.GetValue(second);
                    bool firstValueIsNull = firstValue == null;
                    bool secondValueIsNull = secondValue == null;
                    if(firstValueIsNull && secondValueIsNull)
                        continue;
                    if(firstValueIsNull ^ secondValueIsNull)
                        return false;
                    if(!EqualsInternal(firstValue!, secondValue!))
                        return false;
                }
                localType = localType.BaseType;
            }
            return true;
        }
    }
}
using Moq;

namespace JetDevel.Collections.Tests {
    sealed class AvlBinaryTreeCollectionTests {
        [Test]
        public void Creation() {
            var coll = new AvlBinaryTreeCollection<int>();
            Assert.That(coll.Count, Is.EqualTo(0));
            Assert.That(coll.Root, Is.Null);
        }
        [Test]
        public void Creation1() {
            var coll = new AvlBinaryTreeCollection<int>();
            for(int i = 0; i < 100000; i++) {
                coll.Add(i);
            }
            coll.Clear();
        }
        [Test]
        public void CreateFromEnumerable() {
            var list = new List<int>() { 1, 2, 3, 4 };
            var coll = new AvlBinaryTreeCollection<int>(list);
            Assert.That(coll, Is.EquivalentTo(list));
        }
        [Test]
        public void Clear() {
            var coll = new AvlBinaryTreeCollection<int>() { 1 };
            coll.Clear();
            Assert.That(coll.Count, Is.EqualTo(0));
        }
        [Test]
        public void CopyTo() {
            var coll = new AvlBinaryTreeCollection<int>() { 5, 6 };
            var array = new int[2];
            coll.CopyTo(array, 0);
            Assert.That(array[0], Is.EqualTo(5));
            Assert.That(array[1], Is.EqualTo(6));
        }
        [Test]
        public void CustomComparerTest() {
            var comparerMock = new Moq.Mock<IComparer<int>>();
            comparerMock.Setup(c => c.Compare(It.IsAny<int>(), It.IsAny<int>())).Returns<int, int>((a, b) => b.CompareTo(a));
            var coll = new AvlBinaryTreeCollection<int>(comparerMock.Object) { 5, 6 };
            var result = coll.ToArray();
            Assert.That(result, Is.EqualTo(new[] { 6, 5 }));
        }
        [Test]
        public void First() {
            var coll = new AvlBinaryTreeCollection<int>();
            coll.Clear();
            Assert.That(() => coll.First, Throws.InvalidOperationException);
        }
        [Test]
        public void Last() {
            var coll = new AvlBinaryTreeCollection<int>();
            coll.Clear();
            Assert.That(() => coll.Last, Throws.InvalidOperationException);
        }
        [Test]
        public void IsReadOnly() {
            var coll = new AvlBinaryTreeCollection<int>();
            Assert.That(coll.IsReadOnly, Is.False);
        }
        [Test]
        public void ContainsForEmpty() {
            var coll = new AvlBinaryTreeCollection<int>();
            Assert.That(coll.Contains(0), Is.False);
        }
        [Test]
        public void Add() {
            var coll = new AvlBinaryTreeCollection<int>();
            coll.Add(0);
            Assert.That(coll.Count, Is.EqualTo(1));
            Assert.That(coll.First, Is.EqualTo(0));
            Assert.That(coll.Last, Is.EqualTo(0));
            coll.Add(1);
            Assert.That(coll.First, Is.EqualTo(0));
            Assert.That(coll.Last, Is.EqualTo(1));
            coll.Add(1);
            Assert.That(coll.Count, Is.EqualTo(2));
        }
        [Test]
        public void MaxHeight() {
            var coll = new AvlBinaryTreeCollection<int>();
            coll.Add(0);
            Assert.That(coll.MaxHeight, Is.EqualTo(2));
        }
        [Test]
        public void Remove() {
            var coll = new AvlBinaryTreeCollection<int>();
            coll.Add(0);
            coll.Remove(1);
            Assert.That(coll.Count, Is.EqualTo(1));
        }
        [Test]
        public void GetEnum() {
            var coll = new AvlBinaryTreeCollection<int>();
            coll.Add(0);
            coll.Add(1);
            Assert.That(coll, Is.EquivalentTo(new[] { 0, 1 }));
        }
        [Test]
        public void GetEnumForEmpty() {
            var coll = new AvlBinaryTreeCollection<int>();
            Assert.That(coll, Is.EquivalentTo(new int[] { }));
        }
        [Test]
        public void Add1000() {
            var coll = new AvlBinaryTreeCollection<int>();
            for(int i = 0; i < 1000; i++)
                coll.Add(i);
            Assert.That(coll, Is.EquivalentTo(Enumerable.Range(0, 1000)));
            Assert.That(coll.First, Is.EqualTo(0));
            Assert.That(coll.Last, Is.EqualTo(999));
        }
        [Test]
        public void Add10000() {
            var coll = new AvlBinaryTreeCollection<int>();
            for(int i = 0; i < 10000; i++)
                coll.Add(i);
            for(int i = 0; i < 10000; i++)
                Assert.That(coll.Contains(i), Is.True);
            Assert.That(coll.Count, Is.EqualTo(10000));
            Assert.That(coll.First, Is.EqualTo(0));
            Assert.That(coll.Last, Is.EqualTo(10000 - 1));
            Assert.That(coll, Is.EquivalentTo(Enumerable.Range(0, 10000)));
        }
        [Test]
        public void Add10000AndRemove10000() {
            var coll = new AvlBinaryTreeCollection<int>();
            var list = new List<int>(Enumerable.Range(0, 100000));
            Randomize(list);
            for(int i = 0; i < 100000; i++)
                coll.Add(list[i]);
            Assert.That(coll.Count, Is.EqualTo(100000));
            for(int i = 0; i < 75000; i++)
                Assert.That(coll.Remove(list[i]), Is.True);
            Assert.That(coll.Count, Is.EqualTo(25000));
        }
        [Test]
        public void Height() {
            var coll = new AvlBinaryTreeCollection<int>();
            Assert.That(coll.Height, Is.EqualTo(0));
            coll.Add(0);
            Assert.That(coll.Height, Is.EqualTo(1));
            coll.Add(1);
            Assert.That(coll.Height, Is.EqualTo(2));
            coll.Add(2);
            Assert.That(coll.Height, Is.EqualTo(2));
            coll.Add(3);
            Assert.That(coll.Height, Is.EqualTo(3));
        }
        [Test]
        public void Reverse() {
            var coll = new AvlBinaryTreeCollection<int>();
            var list = new List<int>(Enumerable.Range(0, 100));
            //Randomize(list);
            coll.AddRange(list);
            var reverse = coll.Reverse().ToList();
            reverse.Reverse();
            Assert.That(list, Is.EquivalentTo(reverse));
        }
        [Test]
        public void ReverseEmpty() {
            var coll = new AvlBinaryTreeCollection<int>();
            var list = new List<int>();
            var reverse = coll.Reverse().ToList();
            reverse.Reverse();
            Assert.That(list, Is.EquivalentTo(reverse));
        }

        static void Randomize(List<int> list) {
            var r = new Random((int)DateTime.Now.Ticks);
            if(list.Count == 1)
                return;
            for(int i = list.Count - 1; i > 0; i--) {
                var index = r.Next(i + 1);
                if(index == i)
                    continue;
                var temp = list[i];
                list[i] = list[index];
                list[index] = temp;
            }
        }
    }
}
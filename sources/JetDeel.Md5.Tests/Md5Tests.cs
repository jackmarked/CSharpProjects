namespace JetDevel.Md5.Core.Tests;
sealed class Md5Tests
{
    // private methods...
    void CheckMd5ForString(string message, string digest)
    {
        var actualMd5 = Md5Hash.Calculate(message);
        var expectedMd5 = Md5Hash.Parse(digest);
        Assert.That(actualMd5, Is.EqualTo(expectedMd5), "Md5 hashes are not equlas!");
    }
    void CheckMd5ForBytes(byte[] message, string digest)
    {
        var actualMd5 = Md5Hash.Calculate(message);
        var expectedMd5 = Md5Hash.Parse(digest);
        Assert.That(actualMd5, Is.EqualTo(expectedMd5), "Md5 hashes are not equlas!");
    }

    // tests...
    [Test]
    public void TryParse_Call_ReturnsValidResult()
    {
        Assert.That(!Md5Hash.TryParse("", out var result));
        Assert.That(!Md5Hash.TryParse("d41d8cd98fz0b204e9800998ecf8427e", out _));
        Assert.That(Md5Hash.TryParse("d41d8cd98f00b204e9800998ecf8427e", out var hash));
        Assert.That(hash, Is.EqualTo(new Md5Hash(3649838548, 78774415, 2550759657, 2118318316)));
    }
    [Test]
    public void GetHashCode_Call_ReturnsValidResult()
    {
        // Arrange.
        var hash = new Md5Hash(3649838548, 78774415, 2550759657, 2118318316);

        // Act.
        var code = hash.GetHashCode();

        // Assert.
        Assert.That(code, Is.Not.Zero);
    }
    [Test]
    public void GetLongHashCode_Call_ReturnsValidResult()
    {
        // Arrange.
        var hash = new Md5Hash(3649838548, 78774415, 2550759657, 2118318316);

        // Act.
        var code = hash.GetLongHashCode();

        // Assert.
        Assert.That(code, Is.Not.EqualTo(0));
    }
    [Test]
    public void WriteToStreamAndToByteArrayIsEquivalent()
    {
        // Arrange.
        var hash = new Md5Hash(1, 2, 3, 4);
        var memoryStream = new MemoryStream();

        // Act.
        hash.Write(memoryStream);

        // Assert.
        var arrayFromStram = memoryStream.ToArray();
        var array = hash.ToByteArray();
        CollectionAssert.AreEqual(arrayFromStram, array);
    }
    [Test]
    public void ToByteArray_Call_ReturnsValidResult()
    {
        var hash = new Md5Hash(1, 2, 3, 4);
        var expected = new byte[] { 1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0 };
        var actual = hash.ToByteArray();
        CollectionAssert.AreEqual(expected, actual);
    }
    [Test]
    public void WriteToBinaryWriterAndToByteArrayIsEquivalent()
    {
        var h1 = new Md5Hash(1, 2, 3, 4);
        var memoryStream = new MemoryStream();
        using (var writer = new BinaryWriter(memoryStream))
            h1.Write(writer);
        var arrayFromStram = memoryStream.ToArray();
        var array = h1.ToByteArray();
        CollectionAssert.AreEqual(arrayFromStram, array);
    }
    [Test]
    public void WrittenAndReadIsEquivalent()
    {
        var h1 = new Md5Hash(1, 2, 3, 4);
        var memoryStream = new MemoryStream();
        using (var writer = new BinaryWriter(memoryStream))
            h1.Write(writer);
        var arrayFromStram = memoryStream.ToArray();
        memoryStream = new MemoryStream(arrayFromStram);
        var h2 = new Md5Hash();
        using (var reader = new BinaryReader(memoryStream))
            h2 = Md5Hash.Read(reader);
        Assert.That(h1.A, Is.EqualTo(h2.A));
        Assert.That(h1.B, Is.EqualTo(h2.B));
        Assert.That(h1.C, Is.EqualTo(h2.C));
        Assert.That(h1.D, Is.EqualTo(h2.D));
    }
    [Test]
    public void CompareTo_CallWithEqualHash_ReturnsZero()
    {
        // Arrange.
        var h1 = new Md5Hash(1, 2, 3, 4);
        var h2 = new Md5Hash(1, 2, 3, 4);

        // Act.
        var result = h1.CompareTo(h2);

        // Assert.
        Assert.That(result, Is.Zero);
    }
    [Test]
    public void CompareTo_CallWithDifferentHashes_ReturnsLessThan0()
    {
        // Arrange.
        var h1 = new Md5Hash(1, 2, 3, 3);
        var h2 = new Md5Hash(1, 2, 3, 4);

        // Act.
        var comp = h1.CompareTo(h2);

        // Assert.
        Assert.That(comp, Is.LessThan(0));
    }
    [Test]
    public void CompareToGreaterThan0()
    {
        // Arrange.
        var h1 = new Md5Hash(1, 2, 3, 4);
        var h2 = new Md5Hash(1, 2, 3, 3);

        // Act.
        var comp = h1.CompareTo(h2);

        // Assert.
        Assert.That(comp, Is.GreaterThan(0));
    }
    [Test]
    public void Operators_Call_ReturnsValidResult()
    {
        var h1 = new Md5Hash(1, 2, 3, 4);
        var h2 = new Md5Hash(1, 2, 3, 3);
        Assert.That(h1 > h2);
        Assert.That(h1 != h2);
        Assert.That(h2 < h1);
        Assert.That(new Md5Hash(1, 2, 3, 4) == new Md5Hash(1, 2, 3, 4));
    }
    [Test]
    public void EqualsAsObjectTest()
    {
        var hash1 = new Md5Hash(1, 2, 3, 4);
        var hash2 = new Md5Hash(1, 2, 3, 4);
        var hash3 = new Md5Hash(1, 2, 3, 5);
        object hashAsObject = null;
        Assert.That(!hash1.Equals(hashAsObject));
        hashAsObject = hash2;
        Assert.That(hash1.Equals(hashAsObject));
        hashAsObject = hash3;
        Assert.That(hash1, Is.Not.EqualTo(hashAsObject));
    }
    [Test]
    public void Rfc1321Samples()
    {
        CheckMd5ForString("", "d41d8cd98f00b204e9800998ecf8427e");
        CheckMd5ForString("a", "0cc175b9c0f1b6a831c399e269772661");
        CheckMd5ForString("abc", "900150983cd24fb0d6963f7d28e17f72");
        CheckMd5ForString("message digest", "f96b697d7cb7938d525a2f31aaf161d0");
        CheckMd5ForString("abcdefghijklmnopqrstuvwxyz", "c3fcd3d76192e4007dfb4" +
          "96cca67e13b");
        CheckMd5ForString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxy" +
          "z0123456789", "d174ab98d277d9f5a5611c2c9f419d9f");
        CheckMd5ForString("123456789012345678901234567890123456789012345678901" +
          "23456789012345678901234567890", "57edf4a22be3c955ac49da2e2107b67a");
    }
    [Test]
    public void CalculateForEmptyMessage()
    {
        CheckMd5ForBytes(Array.Empty<byte>(), "d41d8cd98f00b204e9800998ecf8427e");
    }
    [Test]
    public void Md5CompareWithNative()
    {
        const int count = 10000;
        var bytes = new List<byte>(count);
        for (int i = 0; i < count; i++)
        {
            bytes.Add((byte)i);
            var md5 = Md5Hash.Calculate(bytes.ToArray());
            var nativeMd5 = Md5Hash.CalculateNative(bytes.ToArray());
            Assert.That(md5, Is.EqualTo(nativeMd5));
        }
    }
    [Test]
    public void Md5ToByteArrayTest()
    {
        // Arrange.
        var md5 = Md5Hash.Calculate(Array.Empty<byte>());
        string actual = md5.ToString();
        Assert.That(actual, Is.EqualTo("d41d8cd98f00b204e9800998ecf8427e"));

        // Act.
        var bytes = md5.ToByteArray();
        var actualMd5 = new Md5Hash(bytes);

        // Assert.
        Assert.That(actualMd5, Is.EqualTo(md5));
    }
    [Test]
    public void Calculate_CallWith0_ReturnsValidResult()
    {
        var md5 = Md5Hash.Calculate(new byte[1] { 0 });
        string actual = md5.ToString();
        Assert.That(actual, Is.EqualTo("93b885adfe0da089cdf634904fd59f71"));
    }
    [Test]
    public void Calculate_CallForEnumerable_ReturnsValidResult()
    {
        int count = 12345678;
        var list = new List<byte>(count);
        for (int i = 0; i < count; i++)
            list.Add((byte)i);
        var md5 = Md5Hash.Calculate(list);
        string actual = md5.ToString();
        Assert.That(actual, Is.EqualTo("c509d961f3bca4f02402c0e2798a1324"));
    }
    [Test]
    public void Calculate_CalForFile_ReturnsValidResult()
    {
        var name = Path.GetTempFileName();
        var fs = File.Open(name, FileMode.Append);
        var bw = new BinaryWriter(fs);
        for (int i = 0; i < 12345678; i++)
            bw.Write((byte)i);
        fs.Dispose();
        fs = File.Open(name, FileMode.Open);
        var md5 = Md5Hash.Calculate(fs);
        fs.Dispose();
        File.Delete(name);
        string actual = md5.ToString();
        Assert.That(actual, Is.EqualTo("c509d961f3bca4f02402c0e2798a1324"));
    }
    [Test]
    public void PerformanceTest()
    {
        int count = 1000;
        var list = Array.CreateInstance(typeof(byte[]), count) as byte[][];//;//>(count);
        var hList = new Md5Hash[count];
        hList.Initialize();
        var random = new Random();
        for (int i = 0; i < list.Length; i++)
        {
            var bytes = new byte[5];
            random.NextBytes(bytes);
            list[i] = bytes;
        }
        var dateTime = DateTime.Now;
        for (int i = 0; i < hList.Length; i++)
        {
            var hash = Md5Hash.Calculate(list[i]);
            hList[i] = hash;
        }
        var seconds = (DateTime.Now - dateTime).TotalSeconds;
        Console.WriteLine(seconds);
        Console.WriteLine(Math.Round(hList.Length / seconds));
    }
}
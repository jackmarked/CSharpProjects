using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace JetDevel.Md5;
partial struct Md5Hash
{
    // nested types...
    static class NativeCalculator
    {
        static NativeCalculator()
        {
        }
        static readonly MD5 md5 = MD5.Create();
        public static Md5Hash Calculate(Stream stream)
        {
            var bytes = md5.ComputeHash(stream);
            var result = new Md5Hash(bytes);
            return result;
        }
        public static Md5Hash Calculate(byte[] data)
        {
            var bytes = md5.ComputeHash(data);
            var result = new Md5Hash(bytes);
            return result;
        }
        public static Md5Hash Calculate(IEnumerable<byte> bytes)
        {
            using var wrapper = new EnumWrapper(bytes);
            return Calculate(wrapper);
        }

        sealed class EnumWrapper : Stream
        {
            // fields..
            readonly IEnumerator<byte> en;
            private long _Position;

            // public methods...
            public EnumWrapper(IEnumerable<byte> e)
            {
                en = e.GetEnumerator();
            }
            public override int Read(byte[] buffer, int offset, int count)
            {
                int cuurentOffset = offset;
                for (int i = 0; i < count; i++)
                    if (en.MoveNext())
                    {
                        buffer[cuurentOffset++] = en.Current;
                        _Position++;
                    }
                    else
                        return i;
                return count;
            }
            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }
            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }
            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }
            public override void Flush()
            {
            }

            // protected methods...
            protected override void Dispose(bool disposing)
            {
                en.Dispose();
            }

            // properties...
            public override bool CanRead
            {
                get { return true; }
            }
            public override bool CanSeek
            {
                get { return false; }
            }
            public override bool CanWrite
            {
                get { return false; }
            }
            public override long Length
            {
                get { throw new NotImplementedException(); }
            }
            public override long Position
            {
                get
                {
                    return _Position;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
    static class Calculator
    {
        // constants...
        const int bufferSize = 4096;

        public static Md5Hash Calculate(Stream stream)
        {
            var context = new MD5Context();
            context.Initialize();
            var size = stream.Length;
            var savePosition = stream.Position;
            long totalBytes = 0;
            byte[] buffer = GC.AllocateUninitializedArray<byte>(bufferSize, true);// new byte[bufferSize];
            int readBytes;
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                do
                {
                    readBytes = stream.Read(buffer, 0, bufferSize);
                    totalBytes += readBytes;
                    context.Update(buffer, readBytes);
                }
                while (readBytes > 0 && totalBytes < size);
            }
            finally
            {
                try
                {
                    stream.Seek(savePosition, SeekOrigin.Begin);
                }
                catch
                {
                }
            }
            context.Final();
            return context.ToMd5Hash();
        }
        public static Md5Hash Calculate(IEnumerable<byte> bytes)
        {
            var context = new MD5Context();
            context.Initialize();
            var list = new List<byte>(bufferSize);
            using (var enumerator = bytes.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    list.Add(enumerator.Current);
                    if (list.Count == bufferSize)
                    {
                        context.Update(list.ToArray());
                        list = new List<byte>(bufferSize);
                    }
                }
                if (list.Count > 0)
                    context.Update(list.ToArray());
            }
            context.Final();
            return context.ToMd5Hash();
        }
        public static Md5Hash Calculate(byte[] data)
        {
            var context = new MD5Context();
            context.Initialize();
            context.Update(data);
            context.Final();
            return context.ToMd5Hash();
        }

        // nested types...
        unsafe struct MD5Context
        {
            // constants...
            const int S11 = 7;
            const int S12 = 12;
            const int S13 = 17;
            const int S14 = 22;
            const int S21 = 5;
            const int S22 = 9;
            const int S23 = 14;
            const int S24 = 20;
            const int S31 = 4;
            const int S32 = 11;
            const int S33 = 16;
            const int S34 = 23;
            const int S41 = 6;
            const int S42 = 10;
            const int S43 = 15;
            const int S44 = 21;

            // fields...
            uint State0;
            uint State1;
            uint State2;
            uint State3;
            uint Count0;
            uint Count1;
            byte[] Buffer;

            // private methods...
            static uint RotateLeft(uint x, int n)
            {
                return x << n | x >> 32 - n;
            }
            static void FF(ref uint a, uint b, uint c, uint d, uint x, int s, uint Ac)
            {
                a = RotateLeft((b & c | ~b & d) + a + x + Ac, s) + b;
            }
            static void GG(ref uint a, uint b, uint c, uint d, uint x, int s, uint Ac)
            {
                a = RotateLeft((b & d | c & ~d) + a + x + Ac, s) + b;
            }
            static void HH(ref uint a, uint b, uint c, uint d, uint x, int s, uint Ac)
            {
                a = RotateLeft((b ^ c ^ d) + a + x + Ac, s) + b;
            }
            static void II(ref uint a, uint b, uint c, uint d, uint x, int s, uint Ac)
            {
                a = RotateLeft((c ^ (b | ~d)) + a + x + Ac, s) + b;
            }
            void Transform(void* buffer)
            {
                var px = (uint*)buffer;
                var a = State0;
                var b = State1;
                var c = State2;
                var d = State3;
                FF(ref a, b, c, d, *px, S11, 0xd76aa478);
                FF(ref d, a, b, c, px[1], S12, 0xe8c7b756);
                FF(ref c, d, a, b, px[2], S13, 0x242070db);
                FF(ref b, c, d, a, px[3], S14, 0xc1bdceee);
                FF(ref a, b, c, d, px[4], S11, 0xf57c0faf);
                FF(ref d, a, b, c, px[5], S12, 0x4787c62a);
                FF(ref c, d, a, b, px[6], S13, 0xa8304613);
                FF(ref b, c, d, a, px[7], S14, 0xfd469501);
                FF(ref a, b, c, d, px[8], S11, 0x698098d8);
                FF(ref d, a, b, c, px[9], S12, 0x8b44f7af);
                FF(ref c, d, a, b, px[10], S13, 0xffff5bb1);
                FF(ref b, c, d, a, px[11], S14, 0x895cd7be);
                FF(ref a, b, c, d, px[12], S11, 0x6b901122);
                FF(ref d, a, b, c, px[13], S12, 0xfd987193);
                FF(ref c, d, a, b, px[14], S13, 0xa679438e);
                FF(ref b, c, d, a, px[15], S14, 0x49b40821);

                GG(ref a, b, c, d, px[1], S21, 0xf61e2562);
                GG(ref d, a, b, c, px[6], S22, 0xc040b340);
                GG(ref c, d, a, b, px[11], S23, 0x265e5a51);
                GG(ref b, c, d, a, *px, S24, 0xe9b6c7aa);
                GG(ref a, b, c, d, px[5], S21, 0xd62f105d);
                GG(ref d, a, b, c, px[10], S22, 0x2441453);
                GG(ref c, d, a, b, px[15], S23, 0xd8a1e681);
                GG(ref b, c, d, a, px[4], S24, 0xe7d3fbc8);
                GG(ref a, b, c, d, px[9], S21, 0x21e1cde6);
                GG(ref d, a, b, c, px[14], S22, 0xc33707d6);
                GG(ref c, d, a, b, px[3], S23, 0xf4d50d87);
                GG(ref b, c, d, a, px[8], S24, 0x455a14ed);
                GG(ref a, b, c, d, px[13], S21, 0xa9e3e905);
                GG(ref d, a, b, c, px[2], S22, 0xfcefa3f8);
                GG(ref c, d, a, b, px[7], S23, 0x676f02d9);
                GG(ref b, c, d, a, px[12], S24, 0x8d2a4c8a);

                HH(ref a, b, c, d, px[5], S31, 0xfffa3942);
                HH(ref d, a, b, c, px[8], S32, 0x8771f681);
                HH(ref c, d, a, b, px[11], S33, 0x6d9d6122);
                HH(ref b, c, d, a, px[14], S34, 0xfde5380c);
                HH(ref a, b, c, d, px[1], S31, 0xa4beea44);
                HH(ref d, a, b, c, px[4], S32, 0x4bdecfa9);
                HH(ref c, d, a, b, px[7], S33, 0xf6bb4b60);
                HH(ref b, c, d, a, px[10], S34, 0xbebfbc70);
                HH(ref a, b, c, d, px[13], S31, 0x289b7ec6);
                HH(ref d, a, b, c, *px, S32, 0xeaa127fa);
                HH(ref c, d, a, b, px[3], S33, 0xd4ef3085);
                HH(ref b, c, d, a, px[6], S34, 0x4881d05);
                HH(ref a, b, c, d, px[9], S31, 0xd9d4d039);
                HH(ref d, a, b, c, px[12], S32, 0xe6db99e5);
                HH(ref c, d, a, b, px[15], S33, 0x1fa27cf8);
                HH(ref b, c, d, a, px[2], S34, 0xc4ac5665);

                II(ref a, b, c, d, *px, S41, 0xf4292244);
                II(ref d, a, b, c, px[7], S42, 0x432aff97);
                II(ref c, d, a, b, px[14], S43, 0xab9423a7);
                II(ref b, c, d, a, px[5], S44, 0xfc93a039);
                II(ref a, b, c, d, px[12], S41, 0x655b59c3);
                II(ref d, a, b, c, px[3], S42, 0x8f0ccc92);
                II(ref c, d, a, b, px[10], S43, 0xffeff47d);
                II(ref b, c, d, a, px[1], S44, 0x85845dd1);
                II(ref a, b, c, d, px[8], S41, 0x6fa87e4f);
                II(ref d, a, b, c, px[15], S42, 0xfe2ce6e0);
                II(ref c, d, a, b, px[6], S43, 0xa3014314);
                II(ref b, c, d, a, px[13], S44, 0x4e0811a1);
                II(ref a, b, c, d, px[4], S41, 0xf7537e82);
                II(ref d, a, b, c, px[11], S42, 0xbd3af235);
                II(ref c, d, a, b, px[2], S43, 0x2ad7d2bb);
                II(ref b, c, d, a, px[9], S44, 0xeb86d391);
                State0 += a;
                State1 += b;
                State2 += c;
                State3 += d;
            }
            byte[] GetCountBytes()
            {
                ulong result = Count1;
                result = (result << 32) + Count0;
                return BitConverter.GetBytes(result);
            }

            // public methods...
            public Md5Hash ToMd5Hash()
            {
                return new Md5Hash(State0, State1, State2, State3);
            }
            public void Initialize()
            {
                Buffer = new byte[64];
                State0 = 0x67452301;
                State1 = 0xefcdab89;
                State2 = 0x98badcfe;
                State3 = 0x10325476;
            }
            public void Update(byte[] buffer, int inputLength)
            {
                uint increment = (uint)inputLength << 3;
                uint index = Count0 >> 3 & 0x3f;
                Count0 += increment;
                if (Count0 < increment)
                    Count1++;
                Count1 += (uint)inputLength >> 29;
                uint partLength = 64 - index;
                uint i = 0;
                if (inputLength >= partLength)
                {
                    Array.Copy(buffer, 0, Buffer, index, partLength);
                    fixed (void* pBuffer = &Buffer[0])
                        Transform(pBuffer);
                    i = partLength;
                    while (i + 63 < inputLength)
                    {
                        fixed (void* pSubBuffer = &buffer[i])
                            Transform(pSubBuffer);
                        i += 64;
                    }
                    index = 0;
                }
                Array.Copy(buffer, i, Buffer, index, inputLength - i);
            }
            public void Update(byte[] buffer)
            {
                Update(buffer, buffer.Length);
            }
            public void Final()
            {
                uint index = Count0 >> 3 & 0x3f;
                uint padLength = 56 - index;
                if (index > 55)
                    padLength += 64;
                var countBytes = GetCountBytes();
                var padding = new byte[padLength];
                padding[0] = 0x80;
                Update(padding);
                FinalUpdate(countBytes);
            }

            private void FinalUpdate(byte[] buffer)
            {
                var inputLength = buffer.Length;
                uint index = Count0 >> 3 & 0x3f;
                uint partLen = 64 - index;
                if (inputLength < partLen)
                    return;
                Array.Copy(buffer, 0, Buffer, index, partLen);
                fixed (void* pBuffer = &Buffer[0])
                    Transform(pBuffer);
            }
        }
    }
}
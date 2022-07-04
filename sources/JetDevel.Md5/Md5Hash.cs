using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace JetDevel.Md5; 
#if DEBUG
using System.Diagnostics;
[DebuggerDisplay("A:{A}, B:{B}, C:{C}, D:{D}")]
#endif
[StructLayout(LayoutKind.Explicit, Size = Size)]
public readonly partial struct Md5Hash : IComparable<Md5Hash>, IEquatable<Md5Hash> {
    // constants...
    const string STR_Format = "x2";
    public const int Size = 16;

    // fields...
    [FieldOffset(0)]
    private readonly uint a;
    [FieldOffset(4)]
    private readonly uint b;
    [FieldOffset(8)]
    private readonly uint c;
    [FieldOffset(12)]
    private readonly uint d;
    [FieldOffset(0)]
    private readonly ulong ba;
    [FieldOffset(8)]
    private readonly ulong dc;

    // constructors...
    Md5Hash(ulong ba, ulong dc) : this() {
        this.ba = ba;
        this.dc = dc;
    }
    public Md5Hash(uint a, uint b, uint c, uint d) : this() {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
    }
    public Md5Hash(byte[] bytes) : this() {
        a = BitConverter.ToUInt32(bytes, 0);
        b = BitConverter.ToUInt32(bytes, 4);
        c = BitConverter.ToUInt32(bytes, 8);
        d = BitConverter.ToUInt32(bytes, 12);
    }

    // private methods...
    string GetHex(uint value) {
        var bytes = BitConverter.GetBytes(value);
        return string.Concat(bytes[0].ToString(STR_Format),
          bytes[1].ToString(STR_Format),
          bytes[2].ToString(STR_Format),
          bytes[3].ToString(STR_Format));
    }

    // public methods...
    public static Md5Hash Calculate(params byte[] data) {
        return Calculator.Calculate(data);
    }
    public static Md5Hash Calculate(IEnumerable<byte> bytes) {
        return Calculator.Calculate(bytes);
    }
    public static Md5Hash Calculate(Stream stream) {
        return Calculator.Calculate(stream);
    }
    public static Md5Hash Calculate(string s, Encoding encoding) {
        var bytes = encoding.GetBytes(s);
        return Calculator.Calculate(bytes);
    }
    public static Md5Hash Calculate(string s) {
        var bytes = Encoding.Default.GetBytes(s);
        return Calculator.Calculate(bytes);
    }
    public static Md5Hash CalculateNative(params byte[] data) {
        return NativeCalculator.Calculate(data);
    }
    public static Md5Hash CalculateNative(Stream stream) {
        return NativeCalculator.Calculate(stream);
    }
    public static Md5Hash CalculateNative(IEnumerable<byte> bytes) {
        return NativeCalculator.Calculate(bytes);
    }
    public static Md5Hash Parse(string s) {
        var parser = new Parser();
        var validString = Parser.NormalizeString(s);
        if(!Parser.IsValidString(validString))
            throw new FormatException("Wrong md5 hash format");
        return Parser.ParseValidString(validString);
    }
    public static bool TryParse(string s, out Md5Hash result) {
        result = new Md5Hash();
        var parser = new Parser();
        var validString = Parser.NormalizeString(s);
        if(!Parser.IsValidString(validString))
            return false;
        result = Parser.ParseValidString(validString);
        return true;
    }
    public void Write(BinaryWriter writer) {
        writer.Write(ba);
        writer.Write(dc);
    }
    public void Write(Stream output) {
        output.Write(BitConverter.GetBytes(ba), 0, sizeof(ulong));
        output.Write(BitConverter.GetBytes(dc), 0, sizeof(ulong));
    }
    public static Md5Hash Read(BinaryReader reader) {
        var ba = reader.ReadUInt64();
        var dc = reader.ReadUInt64();
        return new Md5Hash(ba, dc);
    }
    public byte[] ToByteArray() {
        var ba = BitConverter.GetBytes(this.ba);
        var dc = BitConverter.GetBytes(this.dc);
        var result = new byte[Size];
        Array.Copy(ba, result, sizeof(ulong));
        Array.Copy(dc, 0, result, sizeof(ulong), sizeof(ulong));
        return result;
    }
    public override bool Equals(object obj) {
        return (obj is Md5Hash hash) && Equals(hash);
    }
    public override int GetHashCode() {
        return HashCode.Combine(a, b, c, d);
    }
    public long GetLongHashCode() {
        var result = ba ^ dc;
        return (long)result;
    }
    public override string ToString() {
        return string.Concat(GetHex(a), GetHex(b), GetHex(c), GetHex(d));
    }

    // public properties...
    public uint A => a;
    public uint B => b;
    public uint C => c;
    public uint D => d;

    #region IComparable<Md5Hash> Members
    public int CompareTo(Md5Hash other) {
        var comp = other.dc;
        if(dc > comp)
            return 1;
        if(dc < comp)
            return -1;
        comp = other.ba;
        if(ba > comp)
            return 1;
        if(ba == comp)
            return 0;
        return -1;
    }
    #endregion
    #region IEquatable<Md5Hash> Members
    public bool Equals(Md5Hash other) => ba == other.ba && dc == other.dc;
    public static bool operator ==(Md5Hash hash1, Md5Hash hash2) => hash1.Equals(hash2);
    public static bool operator !=(Md5Hash hash1, Md5Hash hash2) => !hash1.Equals(hash2);
    public static bool operator >(Md5Hash hash1, Md5Hash hash2) => hash1.CompareTo(hash2) > 0;
    public static bool operator <(Md5Hash hash1, Md5Hash hash2) => hash1.CompareTo(hash2) < 0;
    #endregion
}
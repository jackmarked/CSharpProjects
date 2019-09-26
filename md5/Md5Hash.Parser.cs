using System;
using System.Collections.Generic;

namespace JetDevel.Md5.Core {
    partial struct Md5Hash {
        class Parser {
            const int ZeroCode = '0';
            const int ABaseValue = 87;
            const int Md5StringLength = Md5Hash.Size * 2;

            private byte HexCharToByte(char c) {
                switch(c) {
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                        return (byte)((int)c - ABaseValue);
                }
                return (byte)((int)c - ZeroCode);
            }
            private bool IsHexChar(char c) {
                switch(c) {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                        return true;
                }
                return false;
            }

            public bool IsValidString(string s) {
                if(string.IsNullOrEmpty(s) || s.Length != Md5StringLength)
                    return false;
                var chars = s.ToCharArray();
                for(int i = 0; i < s.Length; i++)
                    if(!IsHexChar(s[i]))
                        return false;
                return true;
            }
            public string NormalizeString(string s) {
                if(string.IsNullOrEmpty(s))
                    return string.Empty;
                if(s.Length < Md5StringLength)
                    return s;
                var result = s.ToLower();
                var charList = new List<char>();
                foreach(var ch in result)
                    if(IsHexChar(ch))
                        charList.Add(ch);
                var length = Math.Min(charList.Count, Md5StringLength);
                return new string(charList.ToArray(), 0, length);
            }
            public Md5Hash ParseValidString(string s) {
                var result = new byte[Md5Hash.Size];
                for(int i = 0; i < Md5Hash.Size; i++)
                    result[i] = (byte)((HexCharToByte(s[i * 2]) << 4) + HexCharToByte(s[i * 2 + 1]));
                return new Md5Hash(result);
            }
        }
    }
}
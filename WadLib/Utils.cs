using System.Text;

namespace WadTool.WadLib
{
    public static class WadUtils
    {
        public static string Decode(byte[] name) => name == null ? null : Encoding.ASCII.GetString(name).Trim('\0').Replace('\0', '.');
        public static byte[] ToShortName(string name, int index = 1, int length = 8)
        {
            name = name.Trim().ToUpper().Replace(" ", String.Empty);//.Split('.')[0];
            //if(name.LastIndexOf('.') >= 0) name = name.Substring(0, name.LastIndexOf('.'));
            byte[] n = Encoding.ASCII.GetBytes(name);
            var newArray = new byte[length];
            if(n.Length > length)
            {
                int len = (int)Math.Floor(Math.Log10(index));
                Array.Copy(n, newArray, (length-2) - len);
                newArray[(length-2) - len] = (byte)'~';
                byte[] ind = Encoding.ASCII.GetBytes(index.ToString());
                Array.Copy(ind, 0, newArray, (length-1) - len, ind.Length);
            }
            else
            {
                Array.Copy(n, newArray, n.Length);
            }
            return newArray;
        }
        public static byte[] ToLongName(string name, int length = 32)
        {
            byte[] n = Encoding.ASCII.GetBytes(name.Trim().Replace('.', '\0'));
            Array.Resize(ref n, length);
            return n;
        }
        public static long ToBlockSize(long size) => (int)Math.Ceiling(size/2048.0) << 11;
        public static void WritePadding(BinaryWriter w, int blocks = 1)
        {
            byte[] pad = new byte[0x800];
            Array.Fill<byte>(pad, 0x34);
            for(int i = 0; i < blocks; i++) w.Write(pad);
        }
    }
    public struct OffsetSize
    {
        internal uint _Value = 0x7FFFF;
        public static implicit operator OffsetSize(uint value) => new OffsetSize(value);
        public static implicit operator uint(OffsetSize value) => value._Value;
        public OffsetSize(uint value = 0x7FFFF) => _Value = value;
        public OffsetSize(uint offset, uint size) => (Offset, Size) = (offset, size);
        public uint Offset {
            get => (_Value & 0x7FFFF) << 11;
            set => _Value = (_Value & 0xFFF80000u) | ((value >> 11) & 0x7FFFFu);
        }
        public uint Size {
            get => (_Value & 0xFFF80000) >> 8;
            set => _Value = (_Value & 0x7FFFFu) | ((value << 8) & 0xFFF80000u);
        }
    }
}
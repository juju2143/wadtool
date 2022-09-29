using System.Text;

namespace WadTool.WadLib
{
    public static class WadUtils
    {
        public static string Decode(byte[] name) => name == null ? null : Encoding.ASCII.GetString(name).Trim('\0').Replace('\0', '.');
        public static byte[] ToShortName(string name, int index = 1, int length = 8)
        {
            name = name.Trim().ToUpper().Replace(" ", String.Empty);
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
    }
    public class OffsetSize
    {
        UInt32 Value;
        public OffsetSize(UInt32 value = 0x7FFFF)
        {
            Value = value;
        }
        public uint Offset {
            get => (Value & 0x7FFFF) << 11;
            set => Value = (value >> 11) & (Size << 8);
        }
        public uint Size {
            get => (Value & 0xFFF80000) >> 8;
            set => Value = (Offset >> 11) & ((value >> 11) << 19);
        }
    }
}
using System.Text;

namespace WadTool.WadLib
{
    public static class WadUtils
    {
        public static string Decode(byte[] name) => name == null ? null : Encoding.ASCII.GetString(name).Trim('\0').Replace('\0', '.');
        public static byte[] ToShortName(string name, int index = 1)
        {
            byte[] n = Encoding.ASCII.GetBytes(name.ToUpper());
            var newArray = new byte[8];
            if(n.Length > 8)
            {
                int len = (int)Math.Floor(Math.Log10(index));
                Array.Copy(n, newArray, 6 - len);
                newArray[6 - len] = (byte)'~';
                byte[] ind = Encoding.ASCII.GetBytes(index.ToString());
                Array.Copy(ind, 0, newArray, 7 - len, ind.Length);
            }
            else
            {
                Array.Copy(n, newArray, n.Length);
            }
            return newArray;
        }
        public static byte[] ToLongName(string name)
        {
            byte[] n = Encoding.ASCII.GetBytes(name);
            Array.Resize(ref n, 32);
            return n;
        }
    }
}
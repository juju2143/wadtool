using WadTool.WadLib;

namespace WadTool
{
    partial class Program
    {
        public static void Replace(FileInfo ind, FileInfo wad, string wadpath, FileInfo filepath)
        {
            var wp = new WadPackage(ind, wad, access: FileAccess.ReadWrite);
            using (FileStream f = filepath.OpenRead())
            {
                wp.WriteFile(wadpath, f);
            }
        }
    }
}
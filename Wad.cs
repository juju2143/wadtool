using System.Text;

namespace WadTool
{
    class WadPackage
    {
        public BinaryReader IndFile;
        public BinaryReader WadFile;
        public FolderInfo Index;
        public WadPackage(string ind, string wad) : this(File.OpenRead(ind), File.OpenRead(wad)) {}
        public WadPackage(FileInfo ind, FileInfo wad) : this(ind.OpenRead(), wad.OpenRead()) {}
        public WadPackage(FileStream ind, FileStream wad) : this(new BinaryReader(ind), new BinaryReader(wad)) {}
        public WadPackage(BinaryReader ind, BinaryReader wad)
        {
            IndFile = ind;
            WadFile = wad;
            Index = new FolderInfo(IndFile, WadFile);
        }
        public static string Decode(byte[] name) => name == null ? null : Encoding.ASCII.GetString(name).Trim('\0').Replace('\0', '.');
        public FileEntry GetFile(string path)
        {
            string[] dirs = path.Split('/');
            FolderEntry node = Index.RootFolder;
            for(int i = 0; !node.IsFileFolder; i++)
            {
                node = node[dirs[i]];
            }
            return node.Files[dirs[dirs.Length-1]];
        }
        public byte[] GetBytes(string path)
        {
            FileEntry f = GetFile(path);
            WadFile.BaseStream.Position = f.Offset;
            return WadFile.ReadBytes((int)f.Size);
        }
    }
}
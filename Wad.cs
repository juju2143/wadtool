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
        public void PrintTree() => PrintTree(Index.RootFolder, "", true);
        public void PrintTree(FolderEntry tree, string indent, bool last, bool first = false)
        {
            if(tree.LongName != null)
                Console.WriteLine("{0}{1}{2} ({3})", indent, last ? "└── " : "├── ", Decode(tree.LongName), Decode(tree.Name));
            else
                Console.WriteLine("{0}{1}{2}", indent, last ? "└── " : "├── ", Decode(tree.Name));
            indent += last ? "    " : "│   ";
            if(tree.IsFileFolder)
            {
                FileList list = tree.Files;
                for(int i = 0; i < list.Files.Count; i++)
                    PrintTree(list.Files[i], indent, i == list.Files.Count - 1);
            }
            else
            {
                for(int i = 0; i < tree.Folders.Count; i++)
                    PrintTree(tree.Folders[i], indent, i == tree.Folders.Count - 1, i == 0);
            }
        }
        public void PrintTree(FileEntry tree, string indent, bool last)
        {
            if(tree.LongName != null)
                Console.WriteLine("{0}{1}{2} ({3})", indent, last ? "└── " : "├── ", Decode(tree.LongName), Decode(tree.Name));
            else
                Console.WriteLine("{0}{1}{2}", indent, last ? "└── " : "├── ", Decode(tree.Name));

            Console.WriteLine("{0}{1}Offset: 0x{2:X}", indent, last ? "    " : "│   ", tree.Offset);
            Console.WriteLine("{0}{1}Size: 0x{2:X}", indent, last ? "    " : "│   ", tree.Size);
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
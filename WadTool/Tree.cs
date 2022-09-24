using WadTool.WadLib;

namespace WadTool
{
    partial class Program
    {
        public static void Tree(FileInfo ind, FileInfo wad)
        {
            var wp = new WadPackage(ind, wad);
            Console.WriteLine(WadUtils.Decode(wp.Index.Name));
            PrintTree(wp.Index.RootFolder);
        }
        public static void PrintTree(FolderEntry tree, string indent = "", bool last = true)
        {
            if(tree.LongName != null)
                Console.WriteLine("{0}{1}{2} ({3})", indent, last ? "└── " : "├── ", WadUtils.Decode(tree.LongName), WadUtils.Decode(tree.ShortName));
            else
                Console.WriteLine("{0}{1}{2}", indent, last ? "└── " : "├── ", WadUtils.Decode(tree.ShortName));
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
                    PrintTree(tree.Folders[i], indent, i == tree.Folders.Count - 1);
            }
        }
        public static void PrintTree(FileEntry tree, string indent, bool last)
        {
            if(tree.LongName != null)
                Console.WriteLine("{0}{1}{2} ({3})", indent, last ? "└── " : "├── ", WadUtils.Decode(tree.LongName), WadUtils.Decode(tree.ShortName));
            else
                Console.WriteLine("{0}{1}{2}", indent, last ? "└── " : "├── ", WadUtils.Decode(tree.ShortName));

            Console.WriteLine("{0}{1}Offset: 0x{2:X}", indent, last ? "    " : "│   ", tree.Offset);
            Console.WriteLine("{0}{1}Size: 0x{2:X}", indent, last ? "    " : "│   ", tree.Size);
        }
    }
}
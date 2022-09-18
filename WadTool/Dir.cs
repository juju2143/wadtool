using WadTool.WadLib;

namespace WadTool
{
    partial class Program
    {
        public static void Dir(FileInfo ind, FileInfo wad, string path)
        {
            var wp = new WadPackage(ind, wad);
            FolderEntry node = wp.Index.RootFolder;

            if(path != null)
            {
                string[] dirs = path.Split('/');
                try
                {
                    foreach (var dir in dirs)
                    {
                        if(!node.IsFileFolder)
                        {
                            node = node[dir];
                        }
                    }
                }
                catch(Exception)
                {
                    Console.Error.WriteLine("{0} not found", path);
                    return;
                }
            }

            Console.WriteLine("Name: {0}", WadUtils.Decode(node.Name));
            if(node.LongName != null) Console.WriteLine("Long name: {0}", WadUtils.Decode(node.LongName));

            if(node.IsFileFolder)
            {
                Console.WriteLine("Children: {0}", node.Files.NumFiles);
                Console.WriteLine("Offset: 0x{0:X} ({0})", node.Offset.Offset);
                Console.WriteLine("Size: 0x{0:X} ({0})", node.Offset.Size);
                Console.WriteLine("Parent index: {0}", node.Index);
                Console.WriteLine();

                for(int i = 0; i < node.Files.NumFiles; i++)
                {
                    FileEntry child = node.Files.Files[i];
                    Console.WriteLine("{0,8:X} {1,8} {2,8} {3}", child.Offset, WadUtils.Decode(child.Name), child.Size, WadUtils.Decode(child.LongName));
                }
            }
            else
            {
                Console.WriteLine("Children: {0}", node.NumChildren);
                Console.WriteLine("First child index: {0}", node.Index);
                Console.WriteLine();

                for(int i = 0; i < node.NumChildren; i++)
                {
                    FolderEntry child = node.Folders[i];
                    if(child.IsFileFolder)
                    {
                        Console.WriteLine("{0,6} {1,8} {2,8:X} {3,8} {4}", child.Index, WadUtils.Decode(child.Name), child.Offset.Offset, child.Offset.Size, WadUtils.Decode(child.LongName));
                    }
                    else
                    {
                        if(child.LongName != null)
                            Console.WriteLine("{0,6} {1,8} {2,17} {3}", child.Index, WadUtils.Decode(child.Name), "", WadUtils.Decode(child.LongName));
                        else
                            Console.WriteLine("{0,6} {1,8}", child.Index, WadUtils.Decode(child.Name));
                    }
                }
            }
        }
    }
}
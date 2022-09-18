using WadTool.WadLib;

namespace WadTool
{
    partial class Program
    {
        public static void Extract(FileInfo ind, FileInfo wad, FileSystemInfo output, bool namelist, string file)
        {
            var wp = new WadPackage(ind, wad);

            if(output == null)
            {
                output = new DirectoryInfo(Directory.GetCurrentDirectory());
            }

            if(file != null)
            {
                byte[] buf = wp.GetBytes(file);
                if(output.Name == "-")
                {
                    using (Stream stdout = Console.OpenStandardOutput())
                    {
                        stdout.Write(buf, 0, buf.Length);
                    }
                }
                else
                {
                    if(output is FileInfo)
                    {
                        using (Stream fileout = ((FileInfo)output).Create())
                        {
                            fileout.Write(buf, 0, buf.Length);
                        }
                    }
                    else
                    {
                        var name = file.Split('/').Last();
                        var f = new FileInfo(Path.Combine(output.FullName, name));

                        using (Stream fileout = f.Create())
                        {
                            fileout.Write(buf, 0, buf.Length);
                        }
                    }
                }
            }
            else
            {
                FolderEntry node = wp.Index.RootFolder;

                if(file != null)
                {
                    string[] dirs = file.Split('/');
                    for(int i = 0; !node.IsFileFolder && i < dirs.Length; i++)
                    {
                        node = node[dirs[i]];
                    }
                }

                DirectoryInfo dir;
                if(output is DirectoryInfo)
                {
                    dir = (DirectoryInfo)output;
                    ExtractTree(wp.WadFile, node, dir, namelist);
                }
                else
                {
                    Console.Error.WriteLine("{0} is not a directory", output.FullName);
                }
            }
        }
        public static void ExtractTree(BinaryReader wad, FolderEntry tree, DirectoryInfo root, bool namelist)
        {
            string name = tree.LongName != null ? WadUtils.Decode(tree.LongName) : WadUtils.Decode(tree.Name);

            DirectoryInfo subdir;
            if(name == "")
                subdir = root;
            else
                subdir = root.CreateSubdirectory(name);

            if(tree.IsFileFolder)
            {
                FileList list = tree.Files;
                for(int i = 0; i < list.Files.Count; i++)
                    if(WadUtils.Decode(list.Files[i].Name) != "NAMELIST" || namelist)
                        ExtractTree(wad, list.Files[i], subdir);
            }
            else
            {
                for(int i = 0; i < tree.Folders.Count; i++)
                    ExtractTree(wad, tree.Folders[i], subdir, namelist);
            }
        }
        public static void ExtractTree(BinaryReader wad, FileEntry tree, DirectoryInfo root)
        {
            string name = tree.LongName != null ? WadUtils.Decode(tree.LongName) : WadUtils.Decode(tree.Name);

            var file = new FileInfo(Path.Combine(root.FullName, name));

            var buf = tree.ReadFile(wad);

            using(Stream fs = file.Create())
            {
                fs.Write(buf, 0, buf.Length);
            }
        }
    }
}
using System.CommandLine;
using WadTool.WadLib;

namespace WadTool
{
    class Program
    {
        public static int Main(string[] argv)
        {
            var rootCommand = new RootCommand("MTVMG/Music 2000 IND/WAD tool");

            var indOption  = new Option<FileInfo>(new string[2]{"-i", "--ind"}, ()=>new FileInfo("/mnt/cdrom/wads/andy.ind"), "IND file");
            var wadOption = new Option<FileInfo>(new string[2]{"-w", "--wad"}, ()=>new FileInfo("/mnt/cdrom/wads/andy.wad"), "WAD file");

            rootCommand.AddGlobalOption(indOption);
            rootCommand.AddGlobalOption(wadOption);

            var treeCommand = new Command("tree", "Print the file tree");
            treeCommand.SetHandler(Tree, indOption, wadOption);
            rootCommand.AddCommand(treeCommand);

            var extractCommand = new Command("extract", "Extract from WAD");
            var outputOption = new Option<FileSystemInfo>(new string[2]{"-o", "--output"}, "Output directory. If not specified, defaults to the current directory. Use - for stdout.") { IsRequired = false };
            var namelistOption = new Option<bool>(new string[2]{"-n", "--namelist"}, "Include namelist");
            var fileArgument = new Argument<string>("file", "File to extract from WAD. With no file specified, extracts the entire directory structure to output directory.") { Arity = ArgumentArity.ZeroOrOne };
            extractCommand.AddOption(outputOption);
            extractCommand.AddOption(namelistOption);
            extractCommand.AddArgument(fileArgument);
            extractCommand.SetHandler(Extract, indOption, wadOption, outputOption, namelistOption, fileArgument);
            rootCommand.AddCommand(extractCommand);

            var dirCommand = new Command("dir", "Directory structure");
            dirCommand.AddAlias("ls");
            var pathArgument = new Argument<string>("path", "File path") { Arity = ArgumentArity.ZeroOrOne };
            dirCommand.AddArgument(pathArgument);
            dirCommand.SetHandler(Dir, indOption, wadOption, pathArgument);
            rootCommand.AddCommand(dirCommand);
            
            return rootCommand.InvokeAsync(argv).Result;
        }
        public static void Tree(FileInfo ind, FileInfo wad)
        {
            var wp = new WadPackage(ind, wad);
            Console.WriteLine(WadPackage.Decode(wp.Index.Name));
            PrintTree(wp.Index.RootFolder);
        }
        public static void PrintTree(FolderEntry tree, string indent = "", bool last = true)
        {
            if(tree.LongName != null)
                Console.WriteLine("{0}{1}{2} ({3})", indent, last ? "└── " : "├── ", WadPackage.Decode(tree.LongName), WadPackage.Decode(tree.Name));
            else
                Console.WriteLine("{0}{1}{2}", indent, last ? "└── " : "├── ", WadPackage.Decode(tree.Name));
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
                Console.WriteLine("{0}{1}{2} ({3})", indent, last ? "└── " : "├── ", WadPackage.Decode(tree.LongName), WadPackage.Decode(tree.Name));
            else
                Console.WriteLine("{0}{1}{2}", indent, last ? "└── " : "├── ", WadPackage.Decode(tree.Name));

            Console.WriteLine("{0}{1}Offset: 0x{2:X}", indent, last ? "    " : "│   ", tree.Offset);
            Console.WriteLine("{0}{1}Size: 0x{2:X}", indent, last ? "    " : "│   ", tree.Size);
        }
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
            string name = tree.LongName != null ? WadPackage.Decode(tree.LongName) : WadPackage.Decode(tree.Name);

            DirectoryInfo subdir;
            if(name == "")
                subdir = root;
            else
                subdir = root.CreateSubdirectory(name);

            if(tree.IsFileFolder)
            {
                FileList list = tree.Files;
                for(int i = 0; i < list.Files.Count; i++)
                    if(WadPackage.Decode(list.Files[i].Name) != "NAMELIST" || namelist)
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
            string name = tree.LongName != null ? WadPackage.Decode(tree.LongName) : WadPackage.Decode(tree.Name);

            var file = new FileInfo(Path.Combine(root.FullName, name));

            var buf = tree.ReadFile(wad);

            using(Stream fs = file.Create())
            {
                fs.Write(buf, 0, buf.Length);
            }
        }
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

            Console.WriteLine("Name: {0}", WadPackage.Decode(node.Name));
            if(node.LongName != null) Console.WriteLine("Long name: {0}", WadPackage.Decode(node.LongName));

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
                    Console.WriteLine("{0,8:X} {1,8} {2,8} {3}", child.Offset, WadPackage.Decode(child.Name), child.Size, WadPackage.Decode(child.LongName));
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
                        Console.WriteLine("{0,6} {1,8} {2,8:X} {3,8} {4}", child.Index, WadPackage.Decode(child.Name), child.Offset.Offset, child.Offset.Size, WadPackage.Decode(child.LongName));
                    }
                    else
                    {
                        if(child.LongName != null)
                            Console.WriteLine("{0,6} {1,8} {2,17} {3}", child.Index, WadPackage.Decode(child.Name), "", WadPackage.Decode(child.LongName));
                        else
                            Console.WriteLine("{0,6} {1,8}", child.Index, WadPackage.Decode(child.Name));
                    }
                }
            }
        }
    }
}

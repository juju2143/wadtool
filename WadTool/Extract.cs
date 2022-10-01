using System.CommandLine;
using WadTool.WadLib;

namespace WadTool
{
    partial class Program
    {
        [CommandSetup]
        public static Command SetupExtract(Option<FileInfo> indOption, Option<FileInfo> wadOption)
        {
            var extractCommand = new Command("extract", "Extract from WAD");
            var outputOption = new Option<FileSystemInfo>(new string[2]{"-o", "--output"}, ()=>new DirectoryInfo(Directory.GetCurrentDirectory()), "Output directory. If not specified, defaults to the current directory. Use - for stdout.") { IsRequired = false };
            var namelistOption = new Option<bool>(new string[2]{"-n", "--namelist"}, "Include namelist");
            var bogusOption = new Option<bool>(new string[2]{"-b", "--no-bogus"}, "Avoid writing bogus files");
            var dryrunOption = new Option<bool>(new string[2]{"-d", "--dry-run"}, "Do not write files at all");
            var fileArgument = new Argument<string>("file", "File to extract from WAD. With no file specified, extracts the entire directory structure to output directory.") { Arity = ArgumentArity.ZeroOrOne };
            extractCommand.AddOption(outputOption);
            extractCommand.AddOption(namelistOption);
            extractCommand.AddOption(bogusOption);
            extractCommand.AddOption(dryrunOption);
            extractCommand.AddArgument(fileArgument);
            extractCommand.SetHandler(Extract, indOption, wadOption, outputOption, namelistOption, bogusOption, dryrunOption, fileArgument);
            return extractCommand;
        }
        public static void Extract(FileInfo ind, FileInfo wad, FileSystemInfo output, bool namelist, bool bogus, bool dryrun, string file)
        {
            var wp = new WadPackage(ind, wad);

            if(file != null)
            {
                FileEntry fe = wp.GetFile(file);
                if(wp.WadFile.Length < fe.Offset+fe.Size)
                {
                    Console.Error.WriteLine("{0} @ 0x{1:X} seems to lie outside the WAD, an empty file will be written unless -b is given", fe.LongName, fe.Offset);
                    if(bogus) return;
                }
                if(fe.Name.EndsWith(".lnk"))
                {
                    Console.Error.WriteLine("{0} @ 0x{1:X} seems to not be legitimate, garbage will be written unless -b is given", fe.LongName, fe.Offset);
                    if(bogus) return;
                }
                byte[] buf = wp.GetBytes(fe);
                if(!dryrun)
                {
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
                    ExtractTree(wp.WadReader, node, dir, namelist, dryrun, bogus);
                }
                else
                {
                    Console.Error.WriteLine("{0} is not a directory", output.FullName);
                }
            }
        }
        public static void ExtractTree(BinaryReader wad, FolderEntry tree, DirectoryInfo root, bool namelist, bool dryrun, bool bogus)
        {
            DirectoryInfo subdir;
            if(tree.Name == "" || dryrun)
                subdir = root;
            else
                subdir = root.CreateSubdirectory(tree.Name);

            if(tree.IsFileFolder)
            {
                FileList list = tree.Files;
                for(int i = 0; i < list.Files.Count; i++)
                    if(list.Files[i].Name != "NAMELIST" || namelist)
                        ExtractTree(wad, list.Files[i], subdir, dryrun, bogus);
            }
            else
            {
                for(int i = 0; i < tree.Folders.Count; i++)
                    ExtractTree(wad, tree.Folders[i], subdir, namelist, dryrun, bogus);
            }
        }
        public static void ExtractTree(BinaryReader wad, FileEntry tree, DirectoryInfo root, bool dryrun, bool bogus)
        {
            if(wad.BaseStream.Length < tree.Offset+tree.Size || tree.Name.EndsWith(".lnk"))
            {
                if(bogus) return;
            }

            var buf = tree.ReadFile(wad);

            if(!dryrun)
            {
                var file = new FileInfo(Path.Combine(root.FullName, tree.Name));
                using(Stream fs = file.Create())
                {
                    fs.Write(buf, 0, buf.Length);
                }
            }
        }
    }
}
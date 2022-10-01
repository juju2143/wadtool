using System.CommandLine;
using WadTool.WadLib;

namespace WadTool
{
    partial class Program
    {
        [CommandSetup]
        public static Command SetupReplace(Option<FileInfo> indOption, Option<FileInfo> wadOption)
        {
            var replaceCommand = new Command("replace", "Replace a file inside the WAD");
            var wadpathArgument = new Argument<string>("WAD path", "Path in the WAD to replace") { Arity = ArgumentArity.ExactlyOne };
            var filepathArgument = new Argument<FileInfo>("file path", "Path in the filesystem to replace file with") { Arity = ArgumentArity.ExactlyOne };
            replaceCommand.AddArgument(wadpathArgument);
            replaceCommand.AddArgument(filepathArgument);
            replaceCommand.SetHandler(Replace, indOption, wadOption, wadpathArgument, filepathArgument);
            return replaceCommand;
        }
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
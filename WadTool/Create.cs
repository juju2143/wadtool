using System.CommandLine;
using WadTool.WadLib;

namespace WadTool
{
    partial class Program
    {
        [CommandSetup]
        public static Command SetupCreate(Option<FileInfo> indOption, Option<FileInfo> wadOption)
        {
            var createCommand = new Command("create", "Create a new WAD from a directory structure");
            var dirpathArgument = new Argument<DirectoryInfo>("path", ()=>new DirectoryInfo(Directory.GetCurrentDirectory()), "Directory structure to create WAD with") { Arity = ArgumentArity.ZeroOrOne };
            createCommand.AddArgument(dirpathArgument);
            createCommand.SetHandler(Create, indOption, wadOption, dirpathArgument);
            return createCommand;
        }
        public static void Create(FileInfo ind, FileInfo wad, DirectoryInfo dir)
        {
            var wp = new WadPackage(ind, wad, dir);
            wp.WriteWad();
        }
    }
}
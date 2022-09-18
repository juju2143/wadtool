using System.CommandLine;

namespace WadTool
{
    partial class Program
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
    }
}

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
            var bogusOption = new Option<bool>(new string[2]{"-b", "--no-bogus"}, "Avoid writing bogus files");
            var dryrunOption = new Option<bool>(new string[2]{"-d", "--dry-run"}, "Do not write files at all");
            var fileArgument = new Argument<string>("file", "File to extract from WAD. With no file specified, extracts the entire directory structure to output directory.") { Arity = ArgumentArity.ZeroOrOne };
            extractCommand.AddOption(outputOption);
            extractCommand.AddOption(namelistOption);
            extractCommand.AddOption(bogusOption);
            extractCommand.AddOption(dryrunOption);
            extractCommand.AddArgument(fileArgument);
            extractCommand.SetHandler(Extract, indOption, wadOption, outputOption, namelistOption, bogusOption, dryrunOption, fileArgument);
            rootCommand.AddCommand(extractCommand);

            var dirCommand = new Command("dir", "Directory structure");
            dirCommand.AddAlias("ls");
            var pathArgument = new Argument<string>("path", "File path") { Arity = ArgumentArity.ZeroOrOne };
            dirCommand.AddArgument(pathArgument);
            dirCommand.SetHandler(Dir, indOption, wadOption, pathArgument);
            rootCommand.AddCommand(dirCommand);

            var replaceCommand = new Command("replace", "Replace a file inside the WAD");
            var wadpathArgument = new Argument<string>("WAD path", "Path in the WAD to replace") { Arity = ArgumentArity.ExactlyOne };
            var filepathArgument = new Argument<FileInfo>("file path", "Path in the filesystem to replace file with") { Arity = ArgumentArity.ExactlyOne };
            replaceCommand.AddArgument(wadpathArgument);
            replaceCommand.AddArgument(filepathArgument);
            replaceCommand.SetHandler(Replace, indOption, wadOption, wadpathArgument, filepathArgument);
            rootCommand.AddCommand(replaceCommand);
            
            return rootCommand.InvokeAsync(argv).Result;
        }
    }
}

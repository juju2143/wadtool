using System.CommandLine;
using System.Reflection;

namespace WadTool
{
    class CommandSetupAttribute : Attribute
    {
        public CommandSetupAttribute() {}
    }
    partial class Program
    {
        public static Func<FileInfo> GetCDROM(string file)
        {
            var firstdrive = DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.CDRom && x.IsReady).FirstOrDefault();
            if(firstdrive == null) return null;
            return ()=>new FileInfo(Path.Combine(firstdrive.RootDirectory.FullName, "wads", file));
        }
        public static int Main(string[] argv)
        {
            var rootCommand = new RootCommand("MTVMG/Music 2000 IND/WAD tool");

            var indOption = new Option<FileInfo>(new string[2]{"-i", "--ind"}, GetCDROM("andy.ind"), "IND file");
            var wadOption = new Option<FileInfo>(new string[2]{"-w", "--wad"}, GetCDROM("andy.wad"), "WAD file");

            rootCommand.AddGlobalOption(indOption);
            rootCommand.AddGlobalOption(wadOption);

            var setupMethods = MethodBase.GetCurrentMethod().DeclaringType.GetMethods().Where(x => x.GetCustomAttributes(typeof(CommandSetupAttribute), false).Any());
            foreach (var method in setupMethods)
            {
                rootCommand.AddCommand((Command)method.Invoke(null, rootCommand.Options.ToArray()));
            }
            
            return rootCommand.InvokeAsync(argv).Result;
        }
    }
}
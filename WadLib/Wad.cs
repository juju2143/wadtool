namespace WadTool.WadLib
{
    public class WadPackage
    {
        public Stream IndFile;
        public Stream WadFile;
        public BinaryReader IndReader;
        public BinaryReader WadReader;
        public BinaryWriter IndWriter;
        public BinaryWriter WadWriter;
        public FolderInfo Index;
        public WadPackage(string ind, string wad, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read) : this(File.Open(ind, mode, access), File.Open(wad, mode, access)) {}
        public WadPackage(FileInfo ind, FileInfo wad, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read) : this(ind.Open(mode, access), wad.Open(mode, access)) {}
        public WadPackage(FileInfo ind, FileInfo wad, DirectoryInfo dir, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite) : this(ind.Open(mode, access), wad.Open(mode, access), dir) {}
        public WadPackage(Stream ind, Stream wad)
        {
            Init(ind, wad);
            Index = new FolderInfo(IndReader, WadReader);
        }
        public WadPackage(Stream ind, Stream wad, DirectoryInfo dir)
        {
            Init(ind, wad);
            Index = CreateFromDir(dir);
        }
        void Init(Stream ind, Stream wad)
        {
            IndFile = ind;
            WadFile = wad;
            IndReader = new BinaryReader(IndFile);
            WadReader = new BinaryReader(WadFile);
            if(IndFile.CanWrite) IndWriter = new BinaryWriter(IndFile);
            if(WadFile.CanWrite) WadWriter = new BinaryWriter(WadFile);
        }
        public FileEntry GetFile(string path)
        {
            string[] dirs = path.Split('/');
            FolderEntry node = Index.RootFolder;
            for(int i = 0; !node.IsFileFolder; i++)
            {
                node = node[dirs[i]];
            }
            return node.Files[dirs[dirs.Length-1]];
        }
        public byte[] GetBytes(FileEntry f)
        {
            WadFile.Position = f.Offset;
            return WadReader.ReadBytes((int)f.Size);
        }
        public byte[] GetBytes(string path)
        {
            FileEntry f = GetFile(path);
            return GetBytes(f);
        }
        public void WriteFile(string path, Stream stream)
        {
            var file = GetFile(path);
            var blocksize = WadUtils.ToBlockSize(file.Size);
            if(stream.Length <= blocksize)
            {
                byte[] clear = new byte[blocksize];
                byte[] size = BitConverter.GetBytes((uint)stream.Length);

                WadFile.Seek(file.Offset, SeekOrigin.Begin);
                WadFile.Write(clear);

                WadFile.Seek(file.Offset, SeekOrigin.Begin);
                stream.CopyTo(WadFile);

                WadFile.Seek(file.Pointer + 12, SeekOrigin.Begin);
                WadFile.Write(size);

                WadFile.Flush();
            }
            else
            {
                throw new IOException("Can't replace in-place yet");
            }
        }
        long IndOffset;
        long WadOffset;
        public FolderInfo CreateFromDir(DirectoryInfo dir)
        {
            IndOffset = 0x30;
            WadOffset = 0;

            var info = dir.EnumerateFileSystemInfos();
            var fi = new FolderInfo();
            var fe = new FolderEntry() {
                Position = IndOffset,
                ShortName = WadUtils.ToShortName(""),
                LongName = WadUtils.ToLongName(""),
                ParentLongName = WadUtils.ToLongName(""),
                Index = -1,
                NumChildren = -32768,
                Offset = 0x7FFFFu,
                Level = 0,
                Folders = new List<FolderEntry>(),
                Path = new List<string>(),
            };
            IndOffset += 0x10;

            fe.Folders = CreateFolderEntries(dir, fe);
            fe.NumChildren = (short)fe.Folders.Count;

            fi.RootFolder = fe;
            fi.FileSize = (uint)IndOffset+4;
            fi.WadSize = WadOffset;

            return fi;
        }
        List<FolderEntry> CreateFolderEntries(DirectoryInfo dir, FolderEntry root)
        {
            var info = dir.EnumerateFileSystemInfos().OrderBy(x => x.Name).ToArray();

            if(info.Length <= 0) return null;

            var path = new List<string>(root.Path);
            path.Add(root.Name);

            if(info[0] is DirectoryInfo)
            {
                info = dir.GetDirectories().OrderBy(x => x.Name).ToArray();

                var output = new List<FolderEntry>(info.Count());
                
                root.Index = (short)((IndOffset - root.Position) >> 4);

                short idx = (short)((root.Position - IndOffset) >> 4);
                foreach (DirectoryInfo item in info)
                {
                    byte[] sname;
                    int i = 0;
                    do 
                    {
                        sname = WadUtils.ToShortName(item.Name, ++i);
                    }
                    while(output.Count(x => x.ShortName.SequenceEqual(sname)) > 0);
                    var fe = new FolderEntry() {
                        Position = IndOffset,
                        ShortName = sname,
                        LongName = WadUtils.ToLongName(item.Name),
                        ParentLongName = root.LongName,
                        Index = idx--,
                        NumChildren = -32768,
                        Offset = 0x7FFFFu,
                        Level = root.Level+1,
                        Folders = new List<FolderEntry>(),
                        Path = path,
                    };
                    IndOffset += 0x10;
                    output.Add(fe);
                }
                for (int i = 0; i < info.Length; i++)
                {
                    output[i].Folders = CreateFolderEntries((DirectoryInfo)info[i], output[i]);
                    if(output[i].Folders != null) output[i].NumChildren = (short)output[i].Folders.Count;
                }
                return output;
            }
            else
            {
                info = dir.GetFiles().Where(x => x.Name != "NAMELIST").OrderBy(x => x.Name).ToArray();
                var fl = new FileList(){
                    Pointer = WadOffset,
                    Name = WadUtils.ToLongName(dir.Name),
                    ParentName = root.ParentLongName,
                    Path = path,
                    NumFiles = (uint)info.Length+1,
                };
                root.Files = fl;
                root.Offset.Offset = (uint)WadOffset;
                root.Offset.Size = ((uint)info.Length+1) << 11;
                var size = 0x10*fl.NumFiles+4;
                WadOffset = WadUtils.ToBlockSize(WadOffset + size);
                fl.Files = new List<FileEntry>(info.Length);

                var longsize = 0x20*(fl.NumFiles+1);
                var namelist = new FileEntry(){
                    Pointer = fl.Pointer+4,
                    Offset = (uint)WadOffset,
                    ShortName = WadUtils.ToShortName("NAMELIST"),
                    LongName = null,
                    Size = longsize,
                    Path = path,
                };
                WadOffset = WadUtils.ToBlockSize(WadOffset + longsize);
                fl.Files.Add(namelist);

                foreach(FileInfo item in info)
                {
                    byte[] sname;
                    int i = 0;
                    do 
                    {
                        sname = WadUtils.ToShortName(item.Name, ++i);
                    }
                    while(fl.Files.Count(x => x.ShortName.SequenceEqual(sname)) > 0);
                    var fe = new FileEntry(){
                        Pointer = fl.Pointer+4+0x10*fl.Files.Count(),
                        Offset = (uint)WadOffset,
                        ShortName = sname,
                        LongName = WadUtils.ToLongName(item.Name),
                        Size = (uint)item.Length,
                        Path = path,
                        File = item,
                    };
                    WadOffset = WadUtils.ToBlockSize(WadOffset + item.Length);
                    fl.Files.Add(fe);
                }

                return null;
            }
        }
        public void WriteWad()
        {
            IndFile.SetLength(0);
            WadFile.SetLength(0);
            IndFile.SetLength(Index.FileSize + 0x1000);
            WadFile.SetLength(Index.WadSize);

            Index.Write(IndWriter);
            WriteFolderWad(Index.RootFolder);
            
            IndFile.Flush();
            WadFile.Flush();
        }
        void WriteFolderWad(FolderEntry folder)
        {
            folder.Write(IndWriter);
            if(folder.Folders != null)
            {
                foreach (var f in folder.Folders)
                {
                    WriteFolderWad(f);
                }
            }
            if(folder.Files != null)
            {
                var files = folder.Files.Files;
                folder.Files.Write(WadWriter);
                foreach (var f in files)
                {
                    f.Write(WadWriter);
                }
                folder.Files.WriteNamelist(WadWriter);
                foreach (var f in files.Where(x => x.Name != "NAMELIST"))
                {
                    f.WriteFile(WadFile);
                }
            }
        }
    }
}
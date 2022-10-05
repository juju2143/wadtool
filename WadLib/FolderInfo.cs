namespace WadTool.WadLib
{
    public class FolderInfo
    {
        public byte[] Name = WadUtils.ToLongName("Music 2"); // 0x20 bytes
        public UInt32 Unknown1 = 0x190000;
        public UInt32 FileSize = 0;
        public UInt32 Unknown2 = 0x17e0;
        public UInt32 Unknown3 = 5;
        public UInt32 Padding = 0x34343434;
        public FolderEntry RootFolder;
        public long WadSize = 0;
        public FolderInfo() {}
        public FolderInfo(byte[] ind, byte[] wad) : this(new MemoryStream(ind), new MemoryStream(wad)) {}
        public FolderInfo(string ind, string wad) : this(File.OpenRead(ind), File.OpenRead(wad)) {}
        public FolderInfo(FileInfo ind, FileInfo wad) : this(ind.OpenRead(), wad.OpenRead()) {}
        public FolderInfo(Stream ind, Stream wad) : this(new BinaryReader(ind), new BinaryReader(wad)) {}
        public FolderInfo(BinaryReader ind, BinaryReader wad)
        {
            Name = ind.ReadBytes(32);
            Unknown1 = ind.ReadUInt32();
            FileSize = ind.ReadUInt32();
            Unknown2 = ind.ReadUInt32();
            Unknown3 = ind.ReadUInt32();
            WadSize = wad.BaseStream.Length;
            RootFolder = new FolderEntry(ind, wad);
        }
        public void Write(BinaryWriter ind)
        {
            ind.BaseStream.Seek(0, SeekOrigin.Begin);
            ind.Write(Name);
            ind.Write(Unknown1);
            ind.Write(FileSize);
            ind.Write(Unknown2);
            ind.Write(Unknown3);
            ind.BaseStream.Seek(FileSize-4, SeekOrigin.Begin);
            ind.Write(Padding);
            WadUtils.WritePadding(ind, 2);
        }
    }
    public class FolderEntry // 16 bytes
    {
        public long Position;
        public string Name
        {
            get => WadUtils.Decode(LongName == null ? ShortName : LongName);
        }
        public byte[] ShortName; // 8 bytes
        public byte[] LongName;
        public byte[] ParentLongName;
        public Int16 Index;
        public Int16 NumChildren;
        public OffsetSize Offset;
        public List<FolderEntry> Folders;
        public FileList Files;
        public uint Level;
        public List<string> Path;
        public bool IsFileFolder {
            get => NumChildren < 0;
        }
        public FolderEntry() {}
        public FolderEntry(byte[] ind, byte[] wad) : this(new MemoryStream(ind), new MemoryStream(wad)) {}
        public FolderEntry(Stream ind, Stream wad) : this(new BinaryReader(ind), new BinaryReader(wad)) {}
        public FolderEntry(BinaryReader ind, BinaryReader wad)
        {
            Read(ind, wad, 0, new List<string>());
        }
        public FolderEntry(BinaryReader ind, BinaryReader wad, long offset, uint level, List<string> path)
        {
            ind.BaseStream.Position = offset;
            Read(ind, wad, level, path);
        }
        void Read(BinaryReader ind, BinaryReader wad, uint level, List<string> path)
        {
            Position = ind.BaseStream.Position;
            ShortName = ind.ReadBytes(8);
            Index = ind.ReadInt16();
            NumChildren = ind.ReadInt16();
            Offset = ind.ReadUInt32();
            Level = level;
            Path = new List<string>(path);
            Path.Add(Name);
            if(!IsFileFolder)
            {
                Folders = new List<FolderEntry>();
                for(short i = Index; i < Index+NumChildren; i++)
                {
                    FolderEntry child = new FolderEntry(ind, wad, Position+16*i, level+1, Path);
                    Folders.Add(child);
                    if(child.ParentLongName != null) LongName = child.ParentLongName;
                }
            }
            else
            {
                Files = new FileList(wad, Offset.Offset, Path);
                LongName = Files.Name;
                ParentLongName = Files.ParentName;
            }
        }
        public FolderEntry this[string index]
        {
            get => Folders.Where(f => WadUtils.Decode(f.ShortName) == index || WadUtils.Decode(f.LongName) == index).Single();
        }
        public void Write(BinaryWriter ind)
        {
            ind.BaseStream.Seek(Position, SeekOrigin.Begin);
            ind.Write(ShortName);
            ind.Write(Index);
            ind.Write(NumChildren);
            ind.Write(Offset);
        }
    }
    public class FileList
    {
        public long Pointer;
        public byte[] Name; // 32 bytes
        public byte[] ParentName; // 32 bytes
        public UInt32 NumFiles;
        public List<FileEntry> Files;
        public List<string> Path;
        public FileList() {}
        public FileList(BinaryReader wad, long offset, List<string> path)
        {
            wad.BaseStream.Position = offset;
            Pointer = offset;
            Path = new List<string>(path);
            NumFiles = wad.ReadUInt32();
            Files = new List<FileEntry>((int)NumFiles);
            for(uint i = 0; i < NumFiles; i++)
            {
                FileEntry file = new FileEntry(wad);
                file.Path = new List<string>(Path);
                Files.Add(file);
            }
            if(NumFiles > 0)
            {
                wad.BaseStream.Position = Files[0].Offset;
                ParentName = wad.ReadBytes(32);
                Name = wad.ReadBytes(32);
                for(int i = 1; i < NumFiles; i++)
                {
                    Files[i].LongName = wad.ReadBytes(32);
                    Files[i].Path.Add(Files[i].Name);
                }
            }
        }
        public FileEntry this[string index]
        {
            get => Files.Where(f => WadUtils.Decode(f.ShortName) == index || WadUtils.Decode(f.LongName) == index).Single();
        }
        public void Write(BinaryWriter wad)
        {
            wad.BaseStream.Seek(Pointer, SeekOrigin.Begin);
            wad.Write(NumFiles);
        }
        public void WriteNamelist(BinaryWriter wad)
        {
            var file = this["NAMELIST"];
            wad.BaseStream.Seek(file.Offset, SeekOrigin.Begin);
            wad.Write(ParentName);
            wad.Write(Name);
            for(int i = 1; i < NumFiles; i++)
            {
                wad.Write(Files[i].LongName);
            }
        }
    }
    public class FileEntry
    {
        public long Pointer;
        public string Name
        {
            get => WadUtils.Decode(LongName == null ? ShortName : LongName);
        }
        public byte[] ShortName; // 8 bytes
        public byte[] LongName; // 32 bytes
        public UInt32 Offset;
        public UInt32 Size;
        public List<string> Path;
        public FileEntry() {}
        public FileEntry(BinaryReader wad)
        {
            Pointer = wad.BaseStream.Position;
            ShortName = wad.ReadBytes(8);
            Offset = wad.ReadUInt32() << 11;
            Size = wad.ReadUInt32();
        }
        public byte[] ReadFile(BinaryReader wad)
        {
            wad.BaseStream.Position = Offset;
            return wad.ReadBytes((int)Size);
        }
        public void Write(BinaryWriter wad)
        {
            wad.BaseStream.Seek(Pointer, SeekOrigin.Begin);
            wad.Write(ShortName);
            wad.Write(Offset >> 11);
            wad.Write(Size);
        }
        public void WriteFile(Stream wad, Stream file)
        {
            wad.Seek(Offset, SeekOrigin.Begin);
            file.CopyTo(wad);
        }
    }
}
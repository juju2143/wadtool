using System.Text;

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
        public WadPackage(string ind, string wad) : this(File.Open(ind, FileMode.OpenOrCreate, FileAccess.ReadWrite), File.Open(wad, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {}
        public WadPackage(FileInfo ind, FileInfo wad) : this(ind.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite), wad.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite)) {}
        public WadPackage(Stream ind, Stream wad)
        {
            IndFile = ind;
            WadFile = wad;
            IndReader = new BinaryReader(IndFile);
            WadReader = new BinaryReader(WadFile);
            IndWriter = new BinaryWriter(IndFile);
            WadWriter = new BinaryWriter(WadFile);
            Index = new FolderInfo(IndReader, WadReader);
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
            int blocksize = (int)Math.Ceiling(file.Size/2048.0)*2048;
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

                WadFile.Seek(file.Pointer + 12, SeekOrigin.Begin);
                WadFile.Write(size);
            }
            else
            {
                throw new IOException("Can't replace in-place yet");
            }
        }
    }
}
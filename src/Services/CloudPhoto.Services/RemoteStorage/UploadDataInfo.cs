namespace CloudPhoto.Services.RemoteStorage
{
    using System.IO;

    public class UploadDataInfo
    {
        public UploadDataInfo(string fileName, MemoryStream stream)
        {
            this.FileName = fileName;
            this.Stream = stream;
        }

        public UploadDataInfo(string fileName, MemoryStream stream, string container, string directoryName)
            : this(fileName, stream)
        {
            this.Container = container;
            this.DirectoryName = directoryName;
        }

        public UploadDataInfo(string fileName, MemoryStream stream, string directoryName)
            : this(fileName, stream)
        {
            this.DirectoryName = directoryName;
        }

        public string FileName { get; }

        public MemoryStream Stream { get; }

        public string Container { get; private set; }

        public string DirectoryName { get; private set; }
    }
}

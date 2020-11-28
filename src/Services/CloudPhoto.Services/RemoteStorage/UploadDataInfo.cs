namespace CloudPhoto.Services.RemoteStorage
{
    using Microsoft.AspNetCore.Http;

    public class UploadDataInfo
    {
        public UploadDataInfo(IFormFile fileInfo)
        {
            this.FileInfo = fileInfo;
        }

        public UploadDataInfo(IFormFile fileInfo, string container, string directoryName)
            : this(fileInfo)
        {
            this.Container = container;
            this.DirectoryName = directoryName;
        }

        public UploadDataInfo(IFormFile fileInfo, string directoryName)
            : this(fileInfo)
        {
            this.DirectoryName = directoryName;
        }

        public IFormFile FileInfo { get; private set; }

        public string Container { get; private set; }

        public string DirectoryName { get; private set; }
    }
}

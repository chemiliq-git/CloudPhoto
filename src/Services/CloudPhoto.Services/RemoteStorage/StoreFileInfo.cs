namespace CloudPhoto.Services.RemoteStorage
{
    public class StoreFileInfo
    {
        public StoreFileInfo(string fileAddress, string fileId)
            : this(true)
        {
            this.FileAddress = fileAddress;
            this.FileId = fileId;
        }

        public StoreFileInfo(bool result)
        {
            this.BoolResult = result;
        }

        public bool BoolResult { get; private set; }

        public string FileAddress { get; private set; }

        public string FileId { get; private set; }
    }
}

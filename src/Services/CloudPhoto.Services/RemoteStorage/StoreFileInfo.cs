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

        public bool BoolResult { get; set; }

        public string FileAddress { get; set; }

        public string FileId { get; set; }
    }
}

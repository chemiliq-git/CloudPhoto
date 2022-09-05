namespace CloudPhoto.Services.RemoteStorage
{
    public class StoreFileInfo
    {
        public StoreFileInfo(string fileAddress, string fileId)
            : this(true)
        {
            FileAddress = fileAddress;
            FileId = fileId;
        }

        public StoreFileInfo(bool result)
        {
            BoolResult = result;
        }

        public bool BoolResult { get; set; }

        public string FileAddress { get; set; }

        public string FileId { get; set; }
    }
}

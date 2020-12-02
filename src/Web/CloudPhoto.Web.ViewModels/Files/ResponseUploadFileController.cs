namespace CloudPhoto.Web.ViewModels.Files
{
    public class ResponseUploadFileController
    {
        public bool Result { get; set; }

        public string ImageUrl { get; set; }

        public string FileId { get; set; }

        public string ErrorMessage { get; set; }
    }
}

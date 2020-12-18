namespace CloudPhoto.Web.ViewModels.Images
{
    public class ImagePagingCookieData
    {
        public string UserId { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public string Type { get; set; }
    }
}

namespace CloudPhoto.Services.ImageValidate
{
    using System.Drawing.Imaging;

    public class ImageValidateResult
    {
        public ImageValidateResult(bool isValid, ImageFormat imageFormat)
        {
            IsValid = isValid;
            ImageFormat = imageFormat;
        }

        public bool IsValid { get; private set; }

        public ImageFormat ImageFormat { get; private set; }
    }
}

namespace CloudPhoto.Services.ImageValidate
{
    using System;
    using System.Drawing.Imaging;
    using System.IO;

    using Microsoft.AspNetCore.Http;

    public class ImageValidator : IImageValidatorService
    {
        public ImageValidateResult ValidateImageFile(IFormFile validateFile)
        {
            try
            {
                if (validateFile == null)
                {
                    return new ImageValidateResult(false, null);
                }

                using var target = new MemoryStream();
                validateFile.CopyTo(target);

                try
                {
                    var img = System.Drawing.Image.FromStream(target);
                    ImageFormat format = GetImageFormat(img);

                    return new ImageValidateResult(true, format);
                }
                catch
                {
                    return new ImageValidateResult(false, null);
                }
            }
            catch (Exception)
            {
                return new ImageValidateResult(false, null);
            }
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/previous-versions/windows/desktop/wiaaut/-wiaaut-consts-formatid
        /// Convert real format to userfrendly format.
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private static ImageFormat GetImageFormat(System.Drawing.Image img)
        {
            ImageFormat format;
            if (img.RawFormat.Guid != null)
            {
                switch (img.RawFormat.Guid.ToString())
                {
                    case "B96B3CAB-0728-11D3-9D7B-0000F81EF32E":
                        {
                            format = ImageFormat.Bmp;
                            break;
                        }

                    case "B96B3CAF-0728-11D3-9D7B-0000F81EF32E":
                        {
                            format = ImageFormat.Png;
                            break;
                        }

                    case "B96B3CB0-0728-11D3-9D7B-0000F81EF32E":
                        {
                            format = ImageFormat.Gif;
                            break;
                        }

                    case "B96B3CAE-0728-11D3-9D7B-0000F81EF32E":
                        {
                            format = ImageFormat.Jpeg;
                            break;
                        }

                    case "B96B3CB1-0728-11D3-9D7B-0000F81EF32E":
                        {
                            format = ImageFormat.Tiff;
                            break;
                        }

                    default:
                        {
                            format = ImageFormat.Jpeg;
                            break;
                        }
                }
            }
            else
            {
                format = img.RawFormat;
            }

            return format;
        }
    }
}

namespace CloudPhoto.Services.Data.BackgroundServices.ImageHelper
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IImageHelper
    {
        Task UploadImage(ImageInfoParams backgroundParams, CancellationToken cancellationToken = default);
    }
}

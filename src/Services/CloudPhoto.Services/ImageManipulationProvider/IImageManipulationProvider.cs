namespace CloudPhoto.Services.ImageManipulationProvider
{
    using System.IO;

    public interface IImageManipulationProvider
    {
        public byte[] Resize(
            MemoryStream fileContents,
            int width,
            int height);
    }
}

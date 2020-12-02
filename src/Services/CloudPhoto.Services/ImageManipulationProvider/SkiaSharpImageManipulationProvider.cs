namespace CloudPhoto.Services.ImageManipulationProvider
{
    using System.IO;

    using SkiaSharp;

    public class SkiaSharpImageManipulationProvider : IImageManipulationProvider
    {
        public byte[] Resize(
            MemoryStream fileContents,
            int maxWidth,
            int maxHeight)
        {
            using SKBitmap sourceBitmap = SKBitmap.Decode(fileContents);

            using SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(maxWidth, maxHeight), SKFilterQuality.Medium);
            using SKImage scaledImage = SKImage.FromBitmap(scaledBitmap);
            using SKData data = scaledImage.Encode();

            return data.ToArray();
        }
    }
}

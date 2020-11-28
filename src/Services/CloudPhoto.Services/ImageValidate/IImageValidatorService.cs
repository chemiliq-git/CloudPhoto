namespace CloudPhoto.Services.ImageValidate
{
    using Microsoft.AspNetCore.Http;

    public interface IImageValidatorService
    {
        ImageValidateResult ValidateImageFile(IFormFile validateFile);
    }
}

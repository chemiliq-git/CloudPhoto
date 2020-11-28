namespace CloudPhoto.Services.Data.ImagiesService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.CategoriesService;

    public class ImagesService : IImagesService
    {
        public ImagesService(
           IDeletableEntityRepository<Image> imageRepository,
           ICategoriesService categoriesService)
        {
            this.ImageRepository = imageRepository;
            this.CategoriesService = categoriesService;
        }

        public IDeletableEntityRepository<Image> ImageRepository { get; }

        public ICategoriesService CategoriesService { get; }

        public async Task<string> CreateAsync(CreateImageModelData createData)
        {
            Category category = this.CategoriesService.GetByCategoryId<Category>(createData.CategoryId);

            var image = new Image
            {
                Title = createData.Title,
                Description = createData.Description,
                AuthorId = createData.AuthorId,
                ImageUrl = createData.ImageUrl,
            };

            image.ImageCategories = new List<ImageCategory>() { new ImageCategory() { CategoryId = category.Id, ImageId = image.Id } };

            await this.ImageRepository.AddAsync(image);

            await this.ImageRepository.SaveChangesAsync();
            return image.Id;
        }

        public Task<bool> Delete(string id)
        {
            throw new System.NotImplementedException();
        }

        public T GetByCategoryId<T>(string categoryId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateAsync(string id, string name, string description)
        {
            throw new System.NotImplementedException();
        }
    }
}

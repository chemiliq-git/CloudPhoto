namespace CloudPhoto.Services.Data.ImagiesService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Services.Data.TagsService;
    using CloudPhoto.Services.Mapping;

    public class ImagesService : IImagesService
    {
        public ImagesService(
           IDeletableEntityRepository<Image> imageRepository,
           ICategoriesService categoriesService,
           ITagsService tagsService)
        {
            this.ImageRepository = imageRepository;
            this.CategoriesService = categoriesService;
            this.TagsService = tagsService;
        }

        public IDeletableEntityRepository<Image> ImageRepository { get; }

        public ICategoriesService CategoriesService { get; }

        public ITagsService TagsService { get; }

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

            image.ImageTags = await this.ParseImageTag(image, createData.Tags);
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

        public IEnumerable<T> GetByFilter<T>(SearchImageData searchData)
        {
            IQueryable<Image> query =
                this.ImageRepository.All();

            if (!string.IsNullOrEmpty(searchData.AuthorId))
            {
                query = query.Where(img => img.AuthorId == searchData.AuthorId);
            }

            return query.To<T>().ToList();
        }

        public Task<bool> UpdateAsync(string id, string name, string description)
        {
            throw new System.NotImplementedException();
        }

        private async Task<ICollection<ImageTag>> ParseImageTag(Image image, List<string> tags)
        {
            List<ImageTag> imageTags = new List<ImageTag>();
            Tag tempTag;

            foreach (string tag in tags)
            {
                tempTag = this.TagsService.GetByTagName<Tag>(tag);
                if (tempTag != null)
                {
                    imageTags.Add(new ImageTag() { ImageId = image.Id, TagId = tempTag.Id });
                }
                else
                {
                    tempTag = new Tag()
                    {
                        Name = tag,
                        Description = tag,
                        AuthorId = image.AuthorId,
                    };

                    tempTag.Id = await this.TagsService.CreateAsync(tag, tag, image.AuthorId);
                    imageTags.Add(new ImageTag() { ImageId = image.Id, TagId = tempTag.Id });
                }
            }

            return imageTags;
        }
    }
}

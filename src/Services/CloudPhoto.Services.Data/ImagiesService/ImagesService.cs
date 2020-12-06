namespace CloudPhoto.Services.Data.ImagiesService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Common;
    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Services.Data.TagsService;
    using CloudPhoto.Services.Data.VotesService;
    using CloudPhoto.Services.ImageManipulationProvider;
    using CloudPhoto.Services.Mapping;
    using CloudPhoto.Services.RemoteStorage;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class ImagesService : IImagesService
    {
        public ImagesService(
            ILogger<ImagesService> logger,
            IDeletableEntityRepository<Image> imageRepository,
            ICategoriesService categoriesService,
            ITagsService tagsService,
            IRemoteStorageService storageService,
            IImageManipulationProvider imageManipulation,
            IConfiguration configuration)
        {
            this.Logger = logger;
            this.ImageRepository = imageRepository;
            this.CategoriesService = categoriesService;
            this.TagsService = tagsService;
            this.ImageManipulation = imageManipulation;
            this.Configuration = configuration;
            this.StorageService = storageService;
        }

        public ILogger<ImagesService> Logger { get; }

        public IDeletableEntityRepository<Image> ImageRepository { get; }

        public ICategoriesService CategoriesService { get; }

        public ITagsService TagsService { get; }

        public IImageManipulationProvider ImageManipulation { get; }

        public IConfiguration Configuration { get; }

        public IRemoteStorageService StorageService { get; }

        public async Task<string> CreateAsync(string rootFolder, CreateImageModelData createData)
        {
            Category category = this.CategoriesService.GetByCategoryId<Category>(createData.CategoryId);

            var image = new Image
            {
                Id = createData.Id,
                Title = createData.Title,
                Description = createData.Description,
                AuthorId = createData.AuthorId,
            };

            image.ThumbnailImageUrl = await this.GenerateThumbnailImage(rootFolder, createData.ImageUrl);

            image.ImageTags = await this.ParseImageTag(image, createData.Tags);
            image.ImageCategories = new List<ImageCategory>() { new ImageCategory() { CategoryId = category.Id, ImageId = image.Id } };

            await this.ImageRepository.AddAsync(image);

            await this.ImageRepository.SaveChangesAsync();

            await this.UploadOriginalFile(createData.Id, rootFolder, createData.ImageUrl);

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

        public IEnumerable<T> GetByFilter<T>(
            SearchImageData searchData,
            int perPage,
            int page = 1)
        {
            IQueryable<Image> query = this.GenerateFilterQuery(searchData, out bool hasAvailableItems);

            if (!hasAvailableItems)
            {
                return new List<T>();
            }

            return query
                .OrderBy(i => i.Id)
                .To<T>()
                .Skip(perPage * (page - 1))
                .Take(perPage).ToList();
        }

        public int GetCountByFilter<T>(SearchImageData searchData)
        {
            IQueryable<Image> query = this.GenerateFilterQuery(searchData, out bool hasAvailableItems);

            if (!hasAvailableItems)
            {
                return 0;
            }

            return query
                .OrderBy(i => i.Id)
                .Count();
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

        private async Task UploadOriginalFile(string id, string rootFolder, string imageUrl)
        {
            try
            {
                string fullPath = rootFolder + imageUrl;
                using (FileStream str = new FileStream(fullPath, FileMode.Open))
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        str.CopyTo(memory);
                        memory.Position = 0;
                        StoreFileInfo storeFile = await this.StorageService.UploadFile(
                            new UploadDataInfo(
                                Path.GetFileName(fullPath),
                                memory,
                                "WebPictures",
                                string.Empty));

                        Image image = this.ImageRepository.All().First(image => image.Id == id);
                        image.ImageUrl = storeFile.FileAddress;
                        this.ImageRepository.Update(image);
                        await this.ImageRepository.SaveChangesAsync();
                    }
                }

                this.DeleteLocalFiles(fullPath);
            }
            catch (Exception e)
            {
                this.Logger.LogError("Error upload original file", e);
            }
        }

        private void DeleteLocalFiles(string fullPath)
        {
            try
            {
                string directoryName = Path.GetDirectoryName(fullPath);
                Directory.Delete(directoryName, true);
            }
            catch (Exception e)
            {
                this.Logger.LogError("Error delete local files", e);
            }
        }

        private async Task<string> GenerateThumbnailImage(string rootFolder, string imageUrl)
        {
            string fullPath = rootFolder + imageUrl;
            using FileStream str = new FileStream(fullPath, FileMode.Open);
            using MemoryStream memory = new MemoryStream();
            str.CopyTo(memory);
            memory.Position = 0;
            byte[] image = this.ImageManipulation.Resize(
                memory,
                int.Parse(this.Configuration.GetSection("Images:ThumbnailImageWidth").Value),
                int.Parse(this.Configuration.GetSection("Images:ThumbnailImageHeight").Value));

            using MemoryStream cropImage = new MemoryStream(image);
            StoreFileInfo storeFile = await this.StorageService.UploadFile(
                new UploadDataInfo(
                    Path.GetFileName(fullPath),
                    cropImage,
                    "WebPictures",
                    string.Empty));
            return storeFile.FileAddress;
        }

        private IQueryable<Image> GenerateFilterQuery(SearchImageData searchData, out bool hasAvailableItem)
        {
            hasAvailableItem = true;
            IQueryable<Image> query =
                            this.ImageRepository.All();

            if (!string.IsNullOrEmpty(searchData.AuthorId))
            {
                query = query.Where(img => img.AuthorId == searchData.AuthorId);
            }

            if (searchData.FilterCategory != null
                && searchData.FilterCategory.Count > 0)
            {
                query = query.Where(img => img.ImageCategories.Where(i => searchData.FilterCategory.Contains(i.CategoryId)).Count() > 0);
            }

            if (!string.IsNullOrEmpty(searchData.FilterByTag))
            {
                var foundTags = this.TagsService.FiterTagsByNames<Tag>(searchData.FilterByTag);
                if (!foundTags.Any())
                {
                    hasAvailableItem = false;
                }

                foreach (Tag tag in foundTags)
                {
                    searchData.FilterTags.Add(tag.Id);
                }
            }

            if (searchData.FilterTags != null
                && searchData.FilterTags.Count > 0)
            {
                query = query.Where(img => img.ImageTags.Where(i => searchData.FilterTags.Contains(i.TagId)).Count() > 0);
            }

            return query;
        }
    }
}

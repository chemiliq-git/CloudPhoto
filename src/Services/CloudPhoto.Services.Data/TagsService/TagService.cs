namespace CloudPhoto.Services.Data.TagsService
{
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Mapping;

    public class TagService : ITagsService
    {
        public TagService(IDeletableEntityRepository<Tag> tagRepository)
        {
            this.TagRepository = tagRepository;
        }

        public IDeletableEntityRepository<Tag> TagRepository { get; }

        public T GetByTagName<T>(string tagName)
        {
            IQueryable<Tag> query =
                this.TagRepository.All()
                .Where(c => c.Name.ToLower() == tagName.ToLower());

            return query.To<T>().FirstOrDefault();
        }

        public async Task<string> CreateAsync(string name, string description, string userId)
        {
            var tag = new Tag
            {
                Name = name,
                Description = description,
                AuthorId = userId,
            };

            await this.TagRepository.AddAsync(tag);
            await this.TagRepository.SaveChangesAsync();
            return tag.Id;
        }
    }
}

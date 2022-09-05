namespace CloudPhoto.Services.Data.TagsService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using Mapping;

    public class TagsService : ITagsService
    {
        public TagsService(IDeletableEntityRepository<Tag> tagRepository)
        {
            TagRepository = tagRepository;
        }

        public IDeletableEntityRepository<Tag> TagRepository { get; }

        public T GetByTagName<T>(string tagName)
        {
            IQueryable<Tag> query =
                TagRepository.All()
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

            await TagRepository.AddAsync(tag);
            await TagRepository.SaveChangesAsync();
            return tag.Id;
        }

        public List<T> FiterTagsByNames<T>(string searchText)
        {
            var query =
                  TagRepository.All()
                  .Where(c => c.Name.Contains(searchText));
            return query.To<T>().ToList();
        }
    }
}

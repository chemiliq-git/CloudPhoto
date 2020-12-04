namespace CloudPhoto.Services.Data.TagsService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITagsService
    {
        public T GetByTagName<T>(string tagName);

        public List<T> FiterTagsByNames<T>(string searchText);

        public Task<string> CreateAsync(string name, string description, string userId);

    }
}

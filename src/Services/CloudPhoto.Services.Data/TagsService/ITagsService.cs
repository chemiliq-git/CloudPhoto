namespace CloudPhoto.Services.Data.TagsService
{
    using System.Threading.Tasks;

    public interface ITagsService
    {
        public T GetByTagName<T>(string tagName);

        public Task<string> CreateAsync(string name, string description, string userId);
    }
}

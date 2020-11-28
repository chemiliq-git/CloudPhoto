namespace CloudPhoto.Services.Data.CategoriesService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICategoriesService
    {
        public Task<string> CreateAsync(string name, string description, string userId);

        public Task<bool> UpdateAsync(string id, string name, string description);

        public Task<bool> Delete(string id);

        public IEnumerable<T> GetAll<T>();

        public T GetByCategoryId<T>(string categoryId);
    }
}

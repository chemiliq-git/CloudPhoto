namespace CloudPhoto.Services.Data.CategoriesService
{
    using System.Collections.Generic;

    public interface ICategoriesService
    {
        public IEnumerable<T> GetAll<T>();

        public T GetByCategoryId<T>(string categoryId);
    }
}

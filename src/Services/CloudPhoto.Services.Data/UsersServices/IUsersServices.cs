namespace CloudPhoto.Services.Data.UsersServices
{
    public interface IUsersServices
    {
        public T GetUserInfo<T>(string userId);
    }
}

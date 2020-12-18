namespace CloudPhoto.Services.Data.UsersServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUsersServices
    {
        public Task<bool> ChangeAvatar(string userId, string avatarId);

        public T GetUserInfo<T>(string infoForUserId, string currentLoginUserId);

        public IEnumerable<T> GetFollowingUsers<T>(string infoForUserId, string currentLoginUserId, int perPage, int page);

        public IEnumerable<T> GetFollowerUsers<T>(string infoForUserId, string currentLoginUserId, int perPage, int page);
    }
}

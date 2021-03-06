﻿namespace CloudPhoto.Services.Data.UsersServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUsersService
    {
        public Task<bool> ChangeAvatar(string userId, string avatarUrl);

        public T GetUserInfo<T>(string infoForUserId, string currentLoginUserId);

        public IEnumerable<T> GetFollowingUsers<T>(string infoForUserId, string currentLoginUserId, int perPage, int page);

        public IEnumerable<T> GetFollowerUsers<T>(string infoForUserId, string currentLoginUserId, int perPage, int page);
    }
}

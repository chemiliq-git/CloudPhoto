﻿namespace CloudPhoto.Services.Data.UsersServices
{
    using System.Threading.Tasks;

    public interface IUsersServices
    {
        public Task<bool> ChangeAvatar(string userId, string avatarId);
    }
}

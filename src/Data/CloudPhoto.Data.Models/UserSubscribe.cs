namespace CloudPhoto.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using CloudPhoto.Data.Common.Models;

    public class UserSubscribe : BaseDeletableModel<string>
    {
        public UserSubscribe()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string UserSubscribedId { get; set; }

        [Required]
        public string SubscribeToUserId { get; set; }

        public virtual ApplicationUser UserSubscribed { get; set; }

        public virtual ApplicationUser SubscribeToUser { get; set; }
    }
}

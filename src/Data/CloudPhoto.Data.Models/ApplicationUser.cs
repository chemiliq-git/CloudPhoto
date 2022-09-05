// ReSharper disable VirtualMemberCallInConstructor
namespace CloudPhoto.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    using CloudPhoto.Data.Common.Models;

    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
            Roles = new HashSet<IdentityUserRole<string>>();
            Claims = new HashSet<IdentityUserClaim<string>>();
            Logins = new HashSet<IdentityUserLogin<string>>();

            Votes = new HashSet<Vote>();
            Images = new HashSet<Image>();
            UserSubscribes = new HashSet<UserSubscribe>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PayPalEmail { get; set; }

        public string UserAvatarUrl { get; set; }

        public string Description { get; set; }

        [NotMapped]
        public virtual string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(FirstName))
                {
                    return FirstName + " " + LastName;
                }
                else
                {
                    return UserName;
                }
            }
        }

        [NotMapped]
        public virtual bool IsFollowCurrentUser { get; set; }

        [NotMapped]
        public virtual int CountFollowers { get; set; }

        [NotMapped]
        public virtual int CountFollowing { get; set; }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public virtual ICollection<Image> Images { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }

        public virtual ICollection<UserSubscribe> UserSubscribes { get; set; }
    }
}
